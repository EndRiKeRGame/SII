using System;
using System.Linq;
using System.Text;
using Common;
using Common.Configs;
using Common.Enums;
using Math.Enums;
using MyNamespace;
using TriInspector;
using Ui;
using UnityEngine;
using VContainer;

namespace Math
{
    public class ItemDistance : MonoBehaviour
    {
        private DebugConsoleView _console;
        private ItemsConfig _itemsConfig;
        private AdvancedHeatmap _heatmap;
        private GridSmoothHeatmap _gridSmoothHeatmap;
        private MatrixTree _matrixTree;

        [Inject]
        public void Construct(
            ItemsConfig itemsConfig,
            DebugConsoleView console,
            AdvancedHeatmap heatmap,
            GridSmoothHeatmap gridSmoothHeatmap,
            MatrixTreeConfig matrixTreeConfig)
        {
            _console = console;
            _itemsConfig = itemsConfig;
            _heatmap = heatmap;
            _gridSmoothHeatmap = gridSmoothHeatmap;
            _matrixTree = new MatrixTree(matrixTreeConfig.RootTag);
            _matrixTree.InitializeFromConfig(matrixTreeConfig);
        }

        // FOR NUMERIC
        private void PrintMatrix(float[][] matrix, string title = "Matrix")
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\n=== {title} ===");
            
            // Добавляем заголовки столбцов
            sb.Append("      ");
            for (int j = 0; j < matrix.Length; j++)
            {
                sb.Append($"[{j:D2}]  ");
            }
            sb.AppendLine();
            
            // Выводим матрицу с номерами строк
            for (int i = 0; i < matrix.Length; ++i)
            {
                sb.Append($"[{i:D2}]  ");
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    sb.Append($"{matrix[i][j]:F2} ");
                }
                sb.AppendLine();
            }
            
            _console.LogMessage(sb.ToString());
            _heatmap.GenerateHeatmap(matrix);
        }
        
        private void PrintArray(float[] array, string title = "Array")
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\n=== {title} ===");
            
            for (int i = 0; i < array.Length; ++i)
            {
                sb.AppendLine($"[{i:D2}] {array[i]:F2}");
            }
            
            _console.LogMessage(sb.ToString());
        }
        
        private float[][] InitializeMatrix(int length)
        {
            float[][] matrix = new float[length][];
            for (var index = 0; index < matrix.Length; index++)
                matrix[index] = new float[length];
            return matrix;
        }

        private float[][] CalculateDistanceMatrix(Func<Item, Item, float> distanceCalculator, string methodName)
        {
            _console.LogMessage($"\n=== Calculating {methodName} ===");
            var items = _itemsConfig.GetAllItems();
            var length = items.Length;

            float[][] matrix = InitializeMatrix(length);

            for (int i = 0; i < length - 1; ++i)
            for (int j = i + 1; j < length; ++j)
            {
                var first = items[i];
                var second = items[j];

                var distance = distanceCalculator(first, second);
                matrix[i][j] = distance;
                matrix[j][i] = distance;
            }

            PrintMatrix(matrix, methodName);
            return matrix;
        }

        [Button]
        public float[][] CalculateEuclidDistances()
        {
            return CalculateDistanceMatrix((first, second) =>
            {
                var evDist = EuclidDistance(
                    new float[] { first.Price, first.AvgPlayTime, first.MinimumAge },
                    new float[] { second.Price, second.AvgPlayTime, second.MinimumAge },
                    new float[]
                    {
                        _itemsConfig.MaxItem.Price, _itemsConfig.MaxItem.AvgPlayTime, _itemsConfig.MaxItem.MinimumAge
                    });
                return Mathf.Lerp(0f, 100f, 1f - evDist);
            }, "Euclidean Distances");
        }

        [Button]
        public float[][] CalculateManhattanDistances()
        {
            return CalculateDistanceMatrix((first, second) =>
            {
                var manhattanDist = ManhattanDistance(
                    new float[] { first.Price, first.AvgPlayTime, first.MinimumAge },
                    new float[] { second.Price, second.AvgPlayTime, second.MinimumAge },
                    new float[]
                    {
                        _itemsConfig.MaxItem.Price, _itemsConfig.MaxItem.AvgPlayTime, _itemsConfig.MaxItem.MinimumAge
                    });
                return Mathf.Lerp(0f, 100f, 1f - manhattanDist);
            }, "Manhattan Distances");
        }

        [Button]
        public float[][] CalculateCosProximity()
        {
            return CalculateDistanceMatrix((first, second) =>
            {
                var cosProximity = CosProximity(
                    new float[] { first.Price, first.AvgPlayTime, first.MinimumAge },
                    new float[] { second.Price, second.AvgPlayTime, second.MinimumAge },
                    new float[]
                    {
                        _itemsConfig.MaxItem.Price, _itemsConfig.MaxItem.AvgPlayTime, _itemsConfig.MaxItem.MinimumAge
                    });
                return Mathf.Lerp(0f, 100f, cosProximity);
            }, "Cosine Proximity");
        }

        [Button]
        public float[][] CalculateJacquardProximity()
        {
            return CalculateDistanceMatrix((first, second) =>
            {
                CountTags(first.Tags, second.Tags, out float both, out float onlyFirst, out float onlySecond);
                var jacquardProximity = Mathf.Lerp(0f, 100f, both / (both + onlyFirst + onlySecond));
                return jacquardProximity;
            }, "Jaccard Proximity");
        }
        
        private float EuclidDistance(float[] first, float[] second, float[] norm)
        {
            var sum = 0f;
            for (int i = 0; i < first.Length; ++i)
            {
                sum += Mathf.Pow(Mathf.Abs(second[i] - first[i]) / norm[i], 2);
            }

            return Mathf.Pow(sum / first.Length, 0.5f);
        }

        private float ManhattanDistance(float[] first, float[] second, float[] norm)
        {
            var sum = 0f;
            for (int i = 0; i < first.Length; ++i)
            {
                sum += Mathf.Abs((second[i] - first[i]) / norm[i]);
            }

            return sum / first.Length;
        }

        private float CosProximity(float[] first, float[] second, float[] norm)
        {
            var sumTop = 0f;
            var sumFirst = 0f;
            var sumSecond = 0f;

            for (int i = 0; i < first.Length; ++i)
            {
                sumTop += (second[i] * first[i]) / Mathf.Pow(norm[i], 2);
                sumFirst += (first[i] * first[i]) / Mathf.Pow(norm[i], 2);
                sumSecond += (second[i] * second[i]) / Mathf.Pow(norm[i], 2);
            }

            return sumTop / (Mathf.Pow(sumFirst, 0.5f) * Mathf.Pow(sumSecond, 0.5f) * first.Length);
        }

        [Button]
        public void CompletedProximity(ProximityTypesForNumeric numericType, ProximityTypesForTypes typesType)
        {
            _console.LogMessage("\n=== Calculating Combined Proximity ===");
            var items = _itemsConfig.GetAllItems();
            var length = items.Length;

            float[][] numMatrix = new float[length][];
            switch (numericType)
            {
                case ProximityTypesForNumeric.Euclid:
                    numMatrix = CalculateEuclidDistances();
                    break;
                case ProximityTypesForNumeric.Manhattan:
                    numMatrix = CalculateManhattanDistances();
                    break;
                case ProximityTypesForNumeric.Cos:
                    numMatrix = CalculateCosProximity();
                    break;
            }

            float[][] typeMatrix = new float[length][];
            switch (typesType)
            {
                case ProximityTypesForTypes.Jacquard:
                    typeMatrix = CalculateJacquardProximity();
                    break;
            }

            var total = SummaryMatrix(numMatrix, typeMatrix);
            PrintMatrix(total, "Combined Proximity Matrix");
        }

        private void CountTags(ItemTag[] first, ItemTag[] second, out float both, out float onlyFirst,
            out float onlySecond)
        {
            both = 0f;
            onlyFirst = 0f;
            onlySecond = 0f;

            foreach (var itemTag in first)
            {
                if (second.Contains(itemTag))
                    both++;
                else
                    onlyFirst++;
            }

            onlySecond = second.Length - both;
        }

        private float[][] SummaryMatrix(
            float[][] numeric,
            float[][] type,
            float kNumeric = 0.3f,
            float kType = 0.7f
        )
        {
            float[][] matrix = new float[numeric.Length][];
            for (var i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new float[matrix.Length];

                for (int j = 0; j < matrix.Length; j++)
                {
                    matrix[i][j] = (numeric[i][j] * kNumeric + type[i][j] * kType) / 2f;
                }
            }

            return matrix;
        }
        private float[] SummaryArray(
            float[] numeric,
            float[] type,
            float kNumeric = 0.3f,
            float kType = 0.7f
        )
        {
            float[] array = new float[numeric.Length];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = numeric[i] * kNumeric + type[i] * kType;
            }

            return array;
        }

        public float[] CalculateEuclidDistancesForOne(Item first)
        {
            _console.LogMessage($"\n=== Calculating Euclidean Distances for {first.Name} ===");
            var items = _itemsConfig.GetAllItems();
            var length = items.Length;
            Item maxProximityItem = new();
            float maxProximity = 0f;

            float[] array = new float[length];

            for (int i = 0; i < length; ++i)
            {
                var second = items[i];
                
                if (first == second)
                {
                    array[i] = 0f;
                    continue;
                }

                var evDist = EuclidDistance(
                    new float[] { first.Price, first.AvgPlayTime, first.MinimumAge },
                    new float[] { second.Price, second.AvgPlayTime, second.MinimumAge },
                    new float[]
                    {
                        _itemsConfig.MaxItem.Price, _itemsConfig.MaxItem.AvgPlayTime, _itemsConfig.MaxItem.MinimumAge
                    });
                
                var distance = Mathf.Lerp(0f, 100f, 1f - evDist);
                array[i] = distance;
                
                if (distance > maxProximity)
                {
                    maxProximity = distance;
                    maxProximityItem = second;
                }
            }
            
            PrintArray(array, $"Distances from {first.Name}");
            _console.LogMessage($"\nMost similar item to {first.Name}: {maxProximityItem.Name} (Distance: {maxProximity:F2})");
            
            return array;
        }
        
        public float[] CalculateJacquardDistancesForOne(Item first)
        {
            _console.LogMessage($"\n=== Calculating Jaccard Distances for {first.Name} ===");
            var items = _itemsConfig.GetAllItems();
            var length = items.Length;
            Item maxProximityItem = new();
            float maxProximity = 0f;

            float[] array = new float[length];

            for (int i = 0; i < length; ++i)
            {
                var second = items[i];
                
                if (first == second)
                    continue;
                
                CountTags(first.Tags, second.Tags, out float both, out float onlyFirst, out float onlySecond);
                var jacquardProximity = Mathf.Lerp(0f, 100f, both / (both + onlyFirst + onlySecond));
                array[i] = jacquardProximity;

                if (jacquardProximity > maxProximity)
                {
                    maxProximity = jacquardProximity;
                    maxProximityItem = second;
                }
            }

            PrintArray(array, $"Jacqard Distances for {first.Name}");
            _console.LogMessage($"Max proximity item for {first.Name}: {maxProximityItem.Name} with proximity {maxProximity:F2}");

            return array;
        }

        public float[] CalculateTreeDistancesForOne(Item first)
        {
            _console.LogMessage($"\n=== Calculating Tree Distances for {first.Name} ===");
            var items = _itemsConfig.GetAllItems();
            var length = items.Length;
            Item maxProximityItem = new();
            float maxProximity = 0f;

            float[] array = new float[length];

            for (int i = 0; i < length; ++i)
            {
                var second = items[i];
                
                if (first == second)
                    continue;

                
                var treeProximity = Mathf.Lerp(0f, 100f, CountTreeWeightForItems(first, second));
                array[i] = treeProximity;

                if (treeProximity > maxProximity)
                {
                    maxProximity = treeProximity;
                    maxProximityItem = second;
                }
            }

            PrintArray(array, $"Jacqard Distances for {first.Name}");
            _console.LogMessage($"Max proximity item for {first.Name}: {maxProximityItem.Name} with proximity {maxProximity:F2}");

            return array;
        }

        private float CountTreeWeightForItems(Item first, Item second)
        {
            float min = 0f;
            float max = 0f;

            foreach (var firstTag in first.Tags)
            {
                float maxEl = 0f;
                float minEl = 10000000f;

                foreach (var secondTag in second.Tags)
                {
                    float dist = _matrixTree.CalculateTotalDistanceFromRoot(firstTag, secondTag);

                    if (dist < minEl)
                        minEl = dist;
                    if (dist > maxEl)
                        maxEl = dist;
                }

                max += maxEl;
                min += minEl;
            }

            return min / max;
        }

        
        public void CompletedProximityForOne(Item item)
        {
            _console.LogMessage("\n=== Calculating Combined Proximity ===");
            var items = _itemsConfig.GetAllItems();
            var length = items.Length;
            float maxValue = 0f;
            int maxValueIndex = 0;

            float[] numArray = CalculateEuclidDistancesForOne(item);
            float[] typeArray = CalculateTreeDistancesForOne(item);

            var total = SummaryArray(numArray, typeArray);
            
            for (int i = 0; i < length; ++i)
            {
                if (total[i] > maxValue)
                {
                    maxValue = total[i];
                    maxValueIndex = i;
                }
            }
            
            PrintArray(total, $"Combined Proximity Array for {item.Name}");
            _console.LogMessage($"\nMost similar item to {item.Name}: {items[maxValueIndex].Name} (Distance: {maxValue:F2})");
        }
    }
}