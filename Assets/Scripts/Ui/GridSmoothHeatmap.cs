using UnityEngine;
using UnityEngine.UI;

namespace Ui
{

    public class GridSmoothHeatmap : MonoBehaviour
    {
        [Header("Heatmap Settings")]
        public int pixelsPerCell = 10; // Количество пикселей на одну ячейку
        public Color minColor = Color.red;
        public Color maxColor = Color.green;
        
        [Header("References")]
        public RawImage heatmapImage;
        
        private Texture2D heatmapTexture;
        private float[][] matrix;

        public void GenerateSmoothHeatmap(float[][] dataMatrix)
        {
            matrix = dataMatrix;
            
            if (matrix == null || matrix.Length == 0) return;
            
            CreateSmoothHeatmapTexture();
            UpdateHeatmapDisplay();
        }

        private void CreateSmoothHeatmapTexture()
        {
            if (heatmapTexture != null)
                DestroyImmediate(heatmapTexture);

            int rows = matrix.Length;
            int cols = matrix[0].Length;
            
            // Создаем текстуру большего размера для плавности
            int textureWidth = cols * pixelsPerCell;
            int textureHeight = rows * pixelsPerCell;
            
            heatmapTexture = new Texture2D(textureWidth, textureHeight);
            heatmapTexture.wrapMode = TextureWrapMode.Clamp;
            heatmapTexture.filterMode = FilterMode.Bilinear;

            // Заполняем текстуру с билинейной интерполяцией
            for (int texY = 0; texY < textureHeight; texY++)
            {
                for (int texX = 0; texX < textureWidth; texX++)
                {
                    // Преобразуем координаты текстуры в координаты матрицы
                    float matrixX = (float)texX / pixelsPerCell;
                    float matrixY = (float)texY / pixelsPerCell;
                    
                    Color pixelColor = GetInterpolatedColor(matrixX, matrixY);
                    heatmapTexture.SetPixel(texX, texY, pixelColor);
                }
            }
            
            heatmapTexture.Apply();
        }

        private Color GetInterpolatedColor(float x, float y)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;
            
            // Ограничиваем координаты
            x = Mathf.Clamp(x, 0, cols - 1);
            y = Mathf.Clamp(y, 0, rows - 1);
            
            // Находим индексы для интерполяции
            int x1 = Mathf.FloorToInt(x);
            int x2 = Mathf.Min(x1 + 1, cols - 1);
            int y1 = Mathf.FloorToInt(y);
            int y2 = Mathf.Min(y1 + 1, rows - 1);
            
            // Вычисляем веса
            float wx = x - x1;
            float wy = y - y1;
            
            // Получаем значения из матрицы
            float v00 = GetMatrixValue(y1, x1);
            float v01 = GetMatrixValue(y1, x2);
            float v10 = GetMatrixValue(y2, x1);
            float v11 = GetMatrixValue(y2, x2);
            
            // Билинейная интерполяция
            float interpolatedValue = 
                v00 * (1 - wx) * (1 - wy) +
                v01 * wx * (1 - wy) +
                v10 * (1 - wx) * wy +
                v11 * wx * wy;
            
            return GetColorForValue(interpolatedValue);
        }

        private float GetMatrixValue(int row, int col)
        {
            if (row >= 0 && row < matrix.Length && col >= 0 && col < matrix[row].Length)
                return matrix[row][col];
            return 0f;
        }

        private Color GetColorForValue(float value)
        {
            float clampedValue = Mathf.Clamp01(value);
            return Color.Lerp(minColor, maxColor, clampedValue);
        }

        private void UpdateHeatmapDisplay()
        {
            if (heatmapImage != null && heatmapTexture != null)
            {
                heatmapImage.texture = heatmapTexture;
                
                // Сохраняем пропорции
                RectTransform rect = heatmapImage.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(heatmapTexture.width, heatmapTexture.height);
            }
        }

        [ContextMenu("Generate Test Heatmap")]
        public void GenerateTestHeatmap()
        {
            float[][] testMatrix = new float[][]
            {
                new float[] { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f },
                new float[] { 0.1f, 0.3f, 0.5f, 0.7f, 0.9f },
                new float[] { 0.2f, 0.4f, 0.6f, 0.8f, 1.0f },
                new float[] { 0.1f, 0.3f, 0.5f, 0.7f, 0.9f }
            };
            
            GenerateSmoothHeatmap(testMatrix);
        }

        void OnDestroy()
        {
            if (heatmapTexture != null)
                DestroyImmediate(heatmapTexture);
        }
    }
}