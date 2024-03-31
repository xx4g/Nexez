using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexez
{
    using Microsoft.DeepDev;
    using Microsoft.ML.OnnxRuntime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Singleton class for managing ONNX inference sessions.
    /// </summary>
    internal class OnnxInferenceSession
    {
        private static readonly Dictionary<string, OnnxInferenceSession> _instances = new Dictionary<string, OnnxInferenceSession>();
        private static readonly object _lock = new object();
        /// <summary>
        /// Gets a singleton instance of OnnxInferenceSession for the specified model path.
        /// </summary>
        /// <param name="modelPath">The path to the ONNX model file.</param>
        /// <returns>An instance of OnnxInferenceSession.</returns>
        internal static OnnxInferenceSession GetInstance(string modelPath)
        {
            lock (_lock)
            {
                if (_instances.TryGetValue(modelPath, out var instance))
                {
                    return instance;
                }
                else
                {
                    instance = new OnnxInferenceSession(modelPath);
                    _instances[modelPath] = instance;
                    return instance;
                }
            }
        }

        private OnnxInferenceSession(string path)
        {
            inferenceSession = new Microsoft.ML.OnnxRuntime.InferenceSession(path);
            _tokenizer = TokenizerBuilder.CreateByModelNameAsync("gpt2");
        }

        /// <summary>
        /// Gets the response from the inference session for the given input text.
        /// </summary>
        /// <param name="text">The input text for which the response is generated.</param>
        /// <returns>A StringBuilder containing the response.</returns>
        internal StringBuilder GetResponse(string text)
        {
            try
            {
                StringBuilder filteredString = new StringBuilder();
                var encoded = _tokenizer.Result.Encode(text, new List<string>());
                var input_ids = encoded.ToArray();
                List<long> inIds = new List<long>();
                // Create a NamedOnnxValue object from the input tensor
                foreach (var id in input_ids)
                {
                    inIds.Add(id);
                }

                // Add the named input value to the list of model inputs
               
                int maxSequenceLength = 100; // maximum sequence length
                                             // Store the initial size of the input sequence
                int initialInputSize = inIds.Count;
                var inputIds = new List<long>(inIds); // Initialize input sequence
                List<string> response = new List<string>();
                ReadOnlySpan<float> startLogits = null;
                ReadOnlySpan<float> endLogits = null;
                float[] tokenActivationOutput = null;
                for (int _i = 0; _i < maxSequenceLength; _i++)
                {
                    // Create input tensor from current input sequence
                    var inputIdsOrtValue = OrtValue.CreateTensorValueFromMemory(inputIds.ToArray(), new long[] { 1, inputIds.Count });

                    // Run inference to predict next token
                    var inputs = new Dictionary<string, OrtValue> { { "input_ids", inputIdsOrtValue }, };

                    var output = inferenceSession.Run(new Microsoft.ML.OnnxRuntime.RunOptions(), inputs, inferenceSession.OutputNames);
                    
                    // Process output tensor to get predicted token
                    tokenActivationOutput = output[0].GetTensorDataAsSpan<float>().ToArray();

                    var logits = tokenActivationOutput.Skip((inputIds.Count - 1) * VOCAB_SIZE).Take(VOCAB_SIZE);
                    float sum = logits.Sum(x => (float)Math.Exp(x));
                    IEnumerable<float> softmax = logits.Select(x => (float)Math.Exp(x) / sum);
                    var predictedToken = softmax.ToList().IndexOf(softmax.Max());

                    if (predictedToken == 198 && _i > initialInputSize)
                    {
                            break;
                    }
                    // Append predicted token to input sequence for next iteration
                    inputIds.Add(predictedToken);
                }
                // Initialize a list to keep track of previously seen sequences of words
                List<string> seenSequences = new List<string>();

                var i__ = 0;
                foreach (var inputId in inputIds)
                {
                    // Check if the current inputId is beyond the initial input size
                    if (initialInputSize < i__)
                    {
                            // This token is part of the generated output, so mark it as such

                            // Decode predicted token
                            var token = _tokenizer.Result.Decode(new int[] { (int)inputId });

                            // Add the token to the current sequence
                            string currentSequence = $"{token} ";
                            Console.WriteLine($"word {token} token {inputId}");
                            // Check if the current sequence is already in the list of seen sequences
                            if (!seenSequences.Contains(currentSequence))
                            {
                                // Append the current sequence to the filtered string
                                filteredString.Append($"{token}");

                                // Add the current sequence to the list of seen sequences
                                seenSequences.Add(currentSequence);
                            }
                        

                    }
                    i__++;
                }
                return filteredString;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new StringBuilder();
            }
        }


        /// <summary>
        /// Represents an instance of the ONNX inference session.
        /// </summary>
        internal Microsoft.ML.OnnxRuntime.InferenceSession inferenceSession = null;

        /// <summary>
        /// Represents a task for tokenization.
        /// </summary>
        internal Task<ITokenizer> _tokenizer = null;

        /// <summary>
        /// The size of the vocabulary.
        /// </summary>
        public const int VOCAB_SIZE = 50257;
    }


}
