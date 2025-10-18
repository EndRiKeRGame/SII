using System;
using System.Collections.Generic;
using System.Linq;
using Common.Configs;
using Common.Enums;
using UnityEngine;

namespace MyNamespace
{
    // Основной класс дерева
    [Serializable]
    public class MatrixTree
    {
        [SerializeField]
        private List<ItemTag> _nodes;
        
        [SerializeField]
        private List<SerializableIntList> _matrix;
        
        [SerializeField]
        private int _rootIndex = 0;

        [Serializable]
        public class SerializableIntList
        {
            public List<int> List = new List<int>();
        }

        public MatrixTree(ItemTag rootTag)
        {
            _nodes = new List<ItemTag> { rootTag };
            _matrix = new List<SerializableIntList>();
            _rootIndex = 0;
        }
        
        public void InitializeFromConfig(MatrixTreeConfig config)
        {
            if (config == null)
            {
                Debug.LogError("TreeConfig is null!");
                return;
            }

            _nodes.Clear();
            _matrix.Clear();

            _nodes.Add(config.RootTag);
            _rootIndex = 0;

            // Добавляем все соединения
            foreach (var connection in config.Connections)
            {
                try
                {
                    AddNode(connection.ParentTag, connection.ChildTag, connection.Weight);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to add connection {connection.ParentTag} -> {connection.ChildTag}: {e.Message}");
                }
            }
            
            AddLeafNode(ItemTag.Варгейм);

            Debug.Log($"Tree initialized with {_nodes.Count} nodes");
        }

        /// <summary>
        /// Добавление новой ноды в дерево
        /// </summary>
        public void AddNode(ItemTag parentTag, ItemTag childTag, int weight = 1)
        {
            if (_nodes.Contains(childTag))
                throw new ArgumentException($"Node with tag '{childTag}' already exists");

            if (!_nodes.Contains(parentTag))
                throw new ArgumentException($"Parent node '{parentTag}' not found");

            // Добавляем новую ноду
            int childIndex = _nodes.Count;
            _nodes.Add(childTag);

            // Расширяем матрицу для новой ноды
            foreach (var row in _matrix)
            {
                row.List.Add(0); // добавляем столбец для существующих строк
            }

            // Добавляем новую строку для новой ноды
            var newRow = new SerializableIntList();
            for (int i = 0; i < _nodes.Count; i++)
            {
                newRow.List.Add(0);
            }
            _matrix.Add(newRow);

            // Устанавливаем связь от родителя к ребенку
            int parentIndex = _nodes.IndexOf(parentTag);
            _matrix[parentIndex].List[childIndex] = weight;
        }
        
        public void AddLeafNode(ItemTag parentTag)
        {
            // Добавляем новую ноду
            int childIndex = _nodes.Count;
            _nodes.Add(parentTag);

            // Расширяем матрицу для новой ноды
            foreach (var row in _matrix)
            {
                row.List.Add(0); // добавляем столбец для существующих строк
            }

            // Добавляем новую строку для новой ноды
            var newRow = new SerializableIntList();
            for (int i = 0; i < _nodes.Count; i++)
            {
                newRow.List.Add(0);
            }
            _matrix.Add(newRow);
        }

        /// <summary>
        /// Получение списка дочерних нод
        /// </summary>
        public List<(ItemTag tag, int weight)> GetChildren(ItemTag nodeTag)
        {
            if (!_nodes.Contains(nodeTag))
                throw new ArgumentException($"Node with tag '{nodeTag}' not found");

            int nodeIndex = _nodes.IndexOf(nodeTag);
            var children = new List<(ItemTag, int)>();

            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_matrix[nodeIndex].List[i] != 0)
                {
                    children.Add((_nodes[i], _matrix[nodeIndex].List[i]));
                }
            }

            return children;
        }

        /// <summary>
        /// Получение родительской ноды
        /// </summary>
        public (ItemTag tag, int weight)? GetParent(ItemTag nodeTag)
        {
            if (!_nodes.Contains(nodeTag))
                throw new ArgumentException($"Node with tag '{nodeTag}' not found");

            int nodeIndex = _nodes.IndexOf(nodeTag);

            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_matrix[i].List[nodeIndex] != 0)
                {
                    return (_nodes[i], _matrix[i].List[nodeIndex]);
                }
            }

            return null; // Корневая нода или ошибка
        }

        /// <summary>
        /// Проверка существования ноды
        /// </summary>
        public bool HasNode(ItemTag nodeTag)
        {
            return _nodes.Contains(nodeTag);
        }

        /// <summary>
        /// Получение всех нод
        /// </summary>
        public List<ItemTag> GetAllNodes()
        {
            return new List<ItemTag>(_nodes);
        }

        /// <summary>
        /// Получение корневой ноды
        /// </summary>
        public ItemTag GetRoot()
        {
            return _nodes[_rootIndex];
        }

        /// <summary>
        /// Визуализация матрицы в консоль (для дебага)
        /// </summary>
        public void PrintMatrix()
        {
            Debug.Log("Tree Matrix:");
            
            string header = "     " + string.Join(" ", _nodes.Select(n => n.ToString().PadRight(8).Substring(0, 5)));
            Debug.Log(header);

            for (int i = 0; i < _nodes.Count; i++)
            {
                string row = $"{_nodes[i].ToString().PadRight(5)} ";
                for (int j = 0; j < _nodes.Count; j++)
                {
                    row += $"{_matrix[i].List[j]}".PadRight(6);
                }
                Debug.Log(row);
            }
        }

        /// <summary>
        /// Получение веса ребра между двумя нодами
        /// </summary>
        public int GetConnectionWeight(ItemTag fromTag, ItemTag toTag)
        {
            if (!_nodes.Contains(fromTag) || !_nodes.Contains(toTag))
                return 0;

            int fromIndex = _nodes.IndexOf(fromTag);
            int toIndex = _nodes.IndexOf(toTag);

            return _matrix[fromIndex].List[toIndex];
        }

        /// <summary>
        /// Проверка существования прямого пути от одной ноды к другой
        /// </summary>
        public bool HasDirectConnection(ItemTag fromTag, ItemTag toTag)
        {
            return GetConnectionWeight(fromTag, toTag) > 0;
        }
        
        /// <summary>
        /// Вычисляет суммарное расстояние от корня до двух нод (сумма весов по пути к каждой ноде)
        /// </summary>
        /// <param name="node1">Первая нода</param>
        /// <param name="node2">Вторая нода</param>
        /// <returns>Сумма расстояний от корня до node1 и от корня до node2, или -1 если путь не найден</returns>
        public int CalculateTotalDistanceFromRoot(ItemTag node1, ItemTag node2)
        {
            if (!HasNode(node1) || !HasNode(node2))
            {
                Debug.LogWarning($"One or both nodes not found: {node1}, {node2}");
                return -1;
            }

            int distance1 = CalculateDistanceFromRoot(node1);
            int distance2 = CalculateDistanceFromRoot(node2);

            if (distance1 == -1 || distance2 == -1)
                return -1;

            return distance1 + distance2;
        }

        /// <summary>
        /// Вычисляет расстояние от корня до указанной ноды (сумма весов по пути)
        /// </summary>
        /// <param name="targetNode">Целевая нода</param>
        /// <returns>Сумма весов по пути от корня до ноды, или -1 если путь не найден</returns>
        public int CalculateDistanceFromRoot(ItemTag targetNode)
        {
            if (!HasNode(targetNode))
            {
                Debug.LogWarning($"Node not found: {targetNode}");
                return -1;
            }

            // Если целевая нода - корень, расстояние 0
            if (targetNode.Equals(GetRoot()))
            {
                return 0;
            }

            // Используем BFS для поиска пути от корня до целевой ноды
            var visited = new HashSet<ItemTag>();
            var queue = new Queue<(ItemTag node, int distance)>();
            
            queue.Enqueue((GetRoot(), 0));
            visited.Add(GetRoot());

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                
                // Если нашли целевую ноду, возвращаем накопленное расстояние
                if (current.node.Equals(targetNode))
                {
                    return current.distance;
                }

                // Добавляем всех детей текущей ноды в очередь
                var children = GetChildren(current.node);
                foreach (var child in children)
                {
                    if (visited.Add(child.tag))
                    {
                        queue.Enqueue((child.tag, current.distance + child.weight));
                    }
                }
            }
            
            // Если путь не найден
            Debug.LogWarning($"Path from root to {targetNode} not found");
            return -1;
        }

        /// <summary>
        /// Находит путь от корня до указанной ноды и возвращает список нод и суммарный вес
        /// </summary>
        /// <param name="targetNode">Целевая нода</param>
        /// <returns>Кортеж (список нод пути, суммарный вес) или null если путь не найден</returns>
        public (List<ItemTag> path, int totalWeight)? FindPathFromRoot(ItemTag targetNode)
        {
            if (!HasNode(targetNode))
            {
                Debug.LogWarning($"Node not found: {targetNode}");
                return null;
            }

            // Для восстановления пути храним информацию о родителях и расстояниях
            var parents = new Dictionary<ItemTag, (ItemTag parent, int weight)>();
            var distances = new Dictionary<ItemTag, int>();
            var visited = new HashSet<ItemTag>();
            var queue = new Queue<ItemTag>();
            
            queue.Enqueue(GetRoot());
            visited.Add(GetRoot());
            distances[GetRoot()] = 0;
            parents[GetRoot()] = (default(ItemTag), 0);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                
                if (current.Equals(targetNode))
                {
                    // Восстанавливаем путь
                    var path = new List<ItemTag>();
                    var currentNode = targetNode;
                    int totalWeight = distances[currentNode];

                    while (!currentNode.Equals(GetRoot()))
                    {
                        path.Insert(0, currentNode);
                        currentNode = parents[currentNode].parent;
                    }
                    path.Insert(0, GetRoot());

                    return (path, totalWeight);
                }

                // Добавляем всех детей текущей ноды в очередь
                var children = GetChildren(current);
                foreach (var child in children)
                {
                    if (!visited.Contains(child.tag))
                    {
                        visited.Add(child.tag);
                        distances[child.tag] = distances[current] + child.weight;
                        parents[child.tag] = (current, child.weight);
                        queue.Enqueue(child.tag);
                    }
                }
            }

            return null;
        }
    }    
}
