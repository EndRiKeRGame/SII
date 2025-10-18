using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Ui
{
    public class AdvancedHeatmap : MonoBehaviour
    {
        [Header("Settings")]
        public int cellSize = 60;
        public int spacing = 2;
        public Color minColor = Color.red;
        public Color maxColor = Color.green;
        public bool showValues = true;
        public float fontSize = 40f;
        
        private float[][] matrix;
        private List<GameObject> cellObjects = new List<GameObject>();

        public void GenerateHeatmap(float[][] dataMatrix)
        {
            matrix = dataMatrix;
            ClearExistingCells();
            
            if (matrix == null || matrix.Length == 0) return;

            // Находим min и max значения для нормализации
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            
            foreach (var row in matrix)
            {
                foreach (var value in row)
                {
                    if (value < minValue) minValue = value;
                    if (value > maxValue) maxValue = value;
                }
            }

            for (int y = 0; y < matrix.Length; y++)
            {
                for (int x = 0; x < matrix[y].Length; x++)
                {
                    CreateCell(x, y, matrix[y][x], minValue, maxValue);
                }
            }
        }

        private void CreateCell(int x, int y, float value, float minVal, float maxVal)
        {
            GameObject cell = new GameObject($"Cell_{x}_{y}");
            cell.transform.SetParent(transform);
            cellObjects.Add(cell);

            // Image компонент
            Image image = cell.AddComponent<Image>();
            
            // Нормализуем значение для цвета
            float normalizedValue = (value - minVal) / (maxVal - minVal);
            image.color = GetColorForValue(normalizedValue);

            // RectTransform
            RectTransform rect = cell.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(cellSize, cellSize);
            rect.anchoredPosition = new Vector2(
                x * (cellSize + spacing), 
                -y * (cellSize + spacing)
            );
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);

            // Текст со значением
            if (showValues)
            {
                GameObject textObj = new GameObject("ValueText");
                textObj.transform.SetParent(cell.transform);
                
                TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
                text.text = value.ToString("F2");
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = fontSize;
                text.fontSizeMax = fontSize;
                text.enableAutoSizing = true;
                text.color = GetContrastColor(image.color);
                
                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.anchoredPosition = Vector2.zero;
            }
        }

        private Color GetColorForValue(float normalizedValue)
        {
            return Color.Lerp(minColor, maxColor, normalizedValue);
        }

        private Color GetContrastColor(Color backgroundColor)
        {
            // Вычисляем яркость цвета и возвращаем черный или белый для контраста
            float brightness = (backgroundColor.r * 0.299f + backgroundColor.g * 0.587f + backgroundColor.b * 0.114f);
            return brightness > 0.5f ? Color.black : Color.white;
        }

        private void ClearExistingCells()
        {
            foreach (GameObject cell in cellObjects)
            {
                if (cell != null)
                    DestroyImmediate(cell);
            }
            cellObjects.Clear();
        }
    }
}