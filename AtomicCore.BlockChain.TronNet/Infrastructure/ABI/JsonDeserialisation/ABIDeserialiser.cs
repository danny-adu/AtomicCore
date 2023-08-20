using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ABIDeserialiser
    /// </summary>
    public class ABIDeserialiser
    {
        /// <summary>
        /// Build Constructor
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public ConstructorABI BuildConstructor(IDictionary<string, object> constructor)
        {
            ConstructorABI constructorABI = new ConstructorABI
            {
                InputParameters = BuildFunctionParameters((List<object>)constructor["inputs"])
            };

            return constructorABI;
        }

        /// <summary>
        /// Build Event
        /// </summary>
        /// <param name="eventobject"></param>
        /// <returns></returns>
        public EventABI BuildEvent(IDictionary<string, object> eventobject)
        {
            EventABI eventABI = new EventABI((string)eventobject["name"], (bool)eventobject["anonymous"])
            {
                InputParameters = BuildEventParameters((List<object>)eventobject["inputs"])
            };

            return eventABI;
        }

        /// <summary>
        /// Build EventParameters
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public Parameter[] BuildEventParameters(List<object> inputs)
        {
            List<Parameter> parameters = new List<Parameter>();
            int parameterOrder = 0;
            foreach (IDictionary<string, object> input in inputs)
            {
                parameterOrder++;

                Parameter parameter = new Parameter((string)input["type"], (string)input["name"], parameterOrder, TryGetInternalType(input))
                {
                    Indexed = (bool)input["indexed"]
                };
                InitialiseTupleComponents(input, parameter);

                parameters.Add(parameter);
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// Initialise TupleComponents
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parameter"></param>
        private void InitialiseTupleComponents(IDictionary<string, object> input, Parameter parameter)
        {
            if (parameter.ABIType is TupleType tupleType)
                tupleType.SetComponents(BuildFunctionParameters((List<object>)input["components"]));

            ArrayType arrayType = parameter.ABIType as ArrayType;

            while (arrayType != null)
                if (arrayType.ElementType is TupleType arrayTupleType)
                {
                    arrayTupleType.SetComponents(BuildFunctionParameters((List<object>)input["components"]));
                    arrayType = null;
                }
                else
                    arrayType = arrayType.ElementType as ArrayType;
        }

        /// <summary>
        /// Build Function
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public FunctionABI BuildFunction(IDictionary<string, object> function)
        {
            bool constant = false;

            if (function.ContainsKey("constant"))
                constant = (bool)function["constant"];
            else
                // for solidity >=0.6.0
                if (function.ContainsKey("stateMutability") && ((string)function["stateMutability"] == "view" || (string)function["stateMutability"] == "pure"))
                constant = true;

            List<object> input_params = new List<object>();
            if (function["inputs"] is JArray input_arrs)
            {
                foreach (var obj in input_arrs)
                {
                    var in_dics = new Dictionary<string, object>();

                    foreach (JProperty pi in obj)
                        in_dics.Add(pi.Name, pi.Value);

                    input_params.Add(in_dics);
                }
            }

            List<object> output_params = new List<object>();
            if (function["outputs"] is JArray output_arrs)
            {
                foreach (var obj in output_arrs)
                {
                    var out_dics = new Dictionary<string, object>();

                    foreach (JProperty pi in obj)
                        out_dics.Add(pi.Name, pi.Value);

                    output_params.Add(out_dics);
                }
            }

            FunctionABI functionABI = new FunctionABI(
                (string)function["name"], constant,
                TryGetSerpentValue(function)
            )
            {
                InputParameters = BuildFunctionParameters(input_params),
                OutputParameters = BuildFunctionParameters(output_params)
            };

            return functionABI;
        }

        /// <summary>
        /// Build FunctionParameters
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public Parameter[] BuildFunctionParameters(List<object> inputs)
        {
            List<Parameter> parameters = new List<Parameter>();

            int parameterOrder = 0;
            foreach (IDictionary<string, object> input in inputs)
            {
                parameterOrder++;

                var parameter = new Parameter(
                    (string)input["type"], 
                    (string)input["name"], 
                    parameterOrder, 
                    TryGetInternalType(input),
                    TryGetSignatureValue(input)
                );

                InitialiseTupleComponents(input, parameter);

                parameters.Add(parameter);
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// Deserialise Contract
        /// </summary>
        /// <param name="abi"></param>
        /// <returns></returns>
        public ContractABI DeserialiseContract(string abi)
        {
            List<IDictionary<string, object>> contract = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IDictionary<string, object>>>(abi);

            return DeserialiseContractBody(contract);
        }

        /// <summary>
        /// Deserialise ContractBody
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        private ContractABI DeserialiseContractBody(List<IDictionary<string, object>> contract)
        {
            List<FunctionABI> functions = new List<FunctionABI>();
            List<EventABI> events = new List<EventABI>();

            ConstructorABI constructor = null;
            foreach (IDictionary<string, object> element in contract)
            {
                if ((string)element["type"] == "function")
                    functions.Add(BuildFunction(element));
                if ((string)element["type"] == "event")
                    events.Add(BuildEvent(element));
                if ((string)element["type"] == "constructor")
                    constructor = BuildConstructor(element);
            }

            ContractABI contractABI = new ContractABI
            {
                Functions = functions.ToArray(),
                Constructor = constructor,
                Events = events.ToArray()
            };

            return contractABI;
        }

        /// <summary>
        /// TryGetSerpentValue
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public bool TryGetSerpentValue(IDictionary<string, object> function)
        {
            try
            {
                if (function.ContainsKey("serpent")) return (bool)function["serpent"];

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// TryGetInternalType
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public string TryGetInternalType(IDictionary<string, object> parameter)
        {
            try
            {
                if (parameter.ContainsKey("internalType")) return (string)parameter["internalType"];

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// TryGetSignatureValue
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public string TryGetSignatureValue(IDictionary<string, object> parameter)
        {
            try
            {
                if (parameter.ContainsKey("signature")) return (string)parameter["signature"];

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}