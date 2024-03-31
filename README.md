# Nexez

Nexez is a C# WPF application designed to facilitate the creation of ONNX models using Python scripts, particularly focused on natural language processing models like GPT-2. It provides an intuitive interface for downloading Python, running scripts to build ONNX models, and interacting with the models through chat.

## Features

1. **Python Installation**: Nexez automatically downloads and sets up Python for running scripts to build ONNX models.
2. **Script Execution**: You can run Python scripts to create ONNX models, such as GPT-2, directly within Nexez.
3. **Model Interaction**: Once the ONNX model is built, you can interact with it through chat in the application.
4. **Script and Library Management**: Nexez allows you to load and save Python scripts for building models and manage lists of Python libraries/modules to install.
5. **Example GPT-2 Model**: The application includes an example of building a GPT-2 model to demonstrate its functionality.

## Usage

1. **Python Setup**:
   - Click on the "Setup Python" button to automatically download and install Python.
   
2. **Script Execution**:
   - Write or load a Python script in the "Python" tab.
   - Click on the "Load Script" button to load an existing script.
   - Click on the "Save Script" button to save the current script.
   - Click on the "Run Script" button to execute the script and build the ONNX model.

3. **Model Interaction**:
   - Once the model is built, switch to the "AI Chat" tab.
   - Enter your input in the textbox provided and click on the "Send" button to chat with the model.

4. **Library Management**:
   - Maintain a list of Python libraries/modules to install for script execution.
   - Click on the "Load Libraries" button to load a list of libraries.
   - Click on the "Save Libraries" button to save the current list of libraries.

## Example

To demonstrate the functionality of Nexez, an example GPT-2 model is provided. Follow these steps:

1. Click on "Setup Python" to ensure Python is installed.

![Nexus Python Page](https://github.com/xx4g/Nexez/assets/52110991/7132491a-58b8-493e-8f96-1f9d2ee9c25b)

3. Load the provided GPT-2 script using the "Load Script" button.

![Run Script](https://github.com/xx4g/Nexez/assets/52110991/9adee473-0379-48d2-8811-ad3ce5a5b9fd)

4. Click on "Run Script" to execute the script and build the GPT-2 model.

![Ai Chat](https://github.com/xx4g/Nexez/assets/52110991/832d546b-71a2-4323-a1b3-38a007ccda30)

5. Switch to the "AI Chat" tab and interact with the GPT-2 model through chat.

## Project References

- **Microsoft.DeepDev.TokenizerLib:** Used for tokenization in natural language processing.
- **Microsoft.ML.OnnxRuntime:** Runtime for ONNX models.
- **Python Deployment:** Required for deploying Python in the application.
- **Python Included:** Included Python runtime.
- **Python.Runtime:** Python runtime for C#.

## Contributions

Contributions to Nexez are welcome! Feel free to submit bug reports, feature requests, or pull requests to help improve the application.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

---

Enjoy building ONNX models and interacting with them through Nexez! If you have any questions or need assistance, don't hesitate to reach out.
