import React from "react";
import { navigate } from "@reach/router";
import net from "../../services/net";
import NavBar from "../../components/NavBar";
import CodeEditor from "../../components/CodeEditor";
import Footer from "../../components/Footer";

interface IProps {}

interface IFile {
  filename: string;
  code: string;
  language: string;
}

interface IState {
  description: string;
  error: string;
  loading: boolean;
  files: IFile[];
}

class NewSnippet extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      description: "",
      error: "",
      loading: false,
      files: [
        {
          filename: "",
          code: "",
          language: "",
        },
      ],
    };
  }

  handleSubmit = async (e: any) => {
    e.preventDefault();
    this.setState({ error: "", loading: false });
    const { description, files } = this.state;
    if (files.length <= 0) {
      this.setState({ error: "You must upload at least 1 file." });
      return;
    }
    var fileError = false;
    files.forEach((file) => {
      if (file.filename === "") {
        this.setState({ error: "The file name cannot be empty." });
        fileError = true;
      }
      if (file.code.replace(/ /g, "") === "") {
        this.setState({ error: "The code cannot be empty." });
        fileError = true;
      }
    });
    if (fileError) return;
    this.setState({ loading: true });
    await net
      .post("/api/snippet/new", {
        description,
        files,
      })
      .then((response) => {
        if (response.data) {
          if (response.data.error) {
            this.setState({
              loading: false,
              error: response.data.error.message,
            });
            return;
          }
          if (response.data.success) {
            navigate("/snippet/" + response.data.guid);
          }
        }
      })
      .catch((reason) => {
        if (
          reason.response &&
          reason.response.data &&
          reason.response.data.error
        ) {
          this.setState({
            loading: false,
            error: reason.response.data.error.message,
          });
        }
      });
  };

  handleRemoveFile = async (id: number) => {
    var { files } = this.state;
    files.splice(id, 1);
    this.setState({
      files: files,
    });
  };

  handleAddFile = async (e: any) => {
    var { files } = this.state;
    files.push({
      filename: "",
      code: "",
      language: "",
    });
    this.setState({
      files: files,
    });
  };

  handleCodeChange = async (id: number, code: string) => {
    var { files } = this.state;
    files[id].code = code;
    this.setState({
      files: files,
    });
  };

  handleFilenameChange = async (id: number, filename: string) => {
    var { files } = this.state;
    files[id].filename = filename;
    files[id].language = this.getLanguage(filename);
    this.setState({
      files: files,
    });
  };

  getLanguage(filename: string) {
    const filenameArray = filename.split(".");
    const extension = filenameArray[filenameArray.length - 1];
    switch (extension) {
      case "cs":
        return "csharp";
      case "css":
        return "css";
      case "md":
        return "markdown";
      case "cpp":
        return "c++";
      case "c":
        return "c";
      case "html":
        return "html";
      case "htm":
        return "html";
      case "js":
        return "javascript";
      case "xml":
        return "xml";
      case "sh":
        return "bash";
      case "go":
        return "go";
      case "rb":
        return "ruby";
      case "java":
        return "java";
      case "json":
        return "json";
      case "lua":
        return "lua";
      case "perl":
        return "perl";
      case "php":
        return "php";
      case "python":
        return "python";
      case "scss":
        return "scss";
      case "sql":
        return "sql";
      case "yml":
        return "yaml";
      case "ts":
        return "typescript";
      case "tsx":
        return "typescript";
      default:
        return "text";
    }
  }

  render() {
    return (
      <div>
        <NavBar />
        <div className="wrapper">
          <div className="auto-overflow">
            <div className="container content">
              {this.state.error && (
                <div className="alert alert-danger" role="alert">
                  {this.state.error}
                </div>
              )}
              <input
                type="text"
                className="form-control"
                placeholder="Description of your snippet..."
                onChange={(e) => this.setState({ description: e.target.value })}
                value={this.state.description}
              />
              {this.state.files.map((file, index) => (
                <CodeEditor
                  key={index}
                  id={index}
                  filename={file.filename}
                  code={file.code}
                  language={file.language}
                  isSingle={this.state.files.length < 2}
                  updateCodeFunc={this.handleCodeChange}
                  updateFilenameFunc={this.handleFilenameChange}
                  removeFileFunc={this.handleRemoveFile}
                />
              ))}
              <div className="d-flex justify-content-between flex-items-center flex-wrap">
                <button
                  type="button"
                  className="btn btn-secondary"
                  onClick={this.handleAddFile}
                >
                  Add file
                </button>
                <button
                  type="button"
                  className="btn btn-primary"
                  onClick={this.handleSubmit}
                >
                  {this.state.loading ? (
                    <div className="spinner-border" role="status">
                      <span className="sr-only">Loading...</span>
                    </div>
                  ) : (
                    "Create snippet"
                  )}
                </button>
              </div>
              <Footer />
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default NewSnippet;
