namespace BoyerMoore;

public class AhoCorasick : ISubstringSearch
    {
        private class Node
        {
            public Dictionary<char, Node> Children { get; } = new Dictionary<char, Node>();
            public Node? Failure { get; set; }
            public List<string> Patterns { get; } = new List<string>();
        }

        private readonly Node root;
        private long stepCount;

        public AhoCorasick(string pattern)
        {
            root = new Node();
            BuildTrie(new List<string> { pattern });
            BuildFailureLinks();
        }

        private void BuildTrie(IEnumerable<string> patterns)
        {
            foreach (var pattern in patterns)
            {
                if (string.IsNullOrEmpty(pattern)) continue;

                var current = root;
                foreach (var c in pattern)
                {
                    stepCount++; // Step: Traversing/Creating child
                    if (!current.Children.ContainsKey(c))
                    {
                        current.Children[c] = new Node();
                    }
                    current = current.Children[c];
                }
                current.Patterns.Add(pattern);
                stepCount++; // Step: Marking end of pattern
            }
        }

        private void BuildFailureLinks()
        {
            var queue = new Queue<Node>();

            foreach (var child in root.Children.Values)
            {
                child.Failure = root;
                queue.Enqueue(child);
            }

            // Build failure links for deeper nodes
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var kvp in current.Children)
                {
                    var character = kvp.Key;
                    var child = kvp.Value;
                    queue.Enqueue(child);

                    var failure = current.Failure;
                    while (failure != null && !failure.Children.ContainsKey(character))
                    {
                        failure = failure.Failure;
                        stepCount++; // Step: Following failure links
                    }

                    child.Failure = failure?.Children.GetValueOrDefault(character) ?? root;

                    // Merge patterns from failure node
                    if (child.Failure.Patterns.Count > 0)
                    {
                        child.Patterns.AddRange(child.Failure.Patterns);
                    }
                    stepCount++; // Step: Next child
                }
            }
        }

        public List<int> Search(string text, string pattern, out long steps)
        {
            // Ensure the Aho-Corasick is initialized with the correct pattern
            stepCount = 0; // Reset step count

            var result = new List<int>();
            var current = root;

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                stepCount++; // Step: Reading character

                // Follow failure links until a match is found or root is reached
                while (current != root && !current.Children.ContainsKey(c))
                {
                    current = current.Failure!;
                    stepCount++; // Step: Following failure link and checking
                }

                if (current.Children.ContainsKey(c))
                {
                    current = current.Children[c];
                }
                else
                {
                    current = root;
                }

                // Check for patterns at the current node
                foreach (var matchedPattern in current.Patterns)
                {
                    if (matchedPattern == pattern)
                    {
                        var position = i - matchedPattern.Length + 1;
                        result.Add(position);
                    }
                    stepCount++;
                }
            }

            steps = stepCount;
            return result;
        }
    }