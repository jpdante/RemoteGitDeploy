import React from "react";
import MonacoEditor from "react-monaco-editor";
import styles from "./codeeditor.module.scss";
import { TrashIcon } from "@primer/octicons-react";

interface IProps {
  id: number;
  filename: string;
  code: string;
  language: string;
  isSingle: boolean;
  updateCodeFunc(id: number, code: string): void;
  updateFilenameFunc(id: number, filename: string): void;
  removeFileFunc(id: number): void;
}

class CodeEditor extends React.Component<IProps> {
  handleRemoveFile = async (e: any) => {
    this.props.removeFileFunc(this.props.id);
  };

  render() {
    return (
      <div className={`card ${styles.newSnippet}`}>
        <div className="card-header">
          <div className="input-group">
            <input
              type="text"
              className="form-control"
              placeholder="Filename with extension..."
              onChange={(e) => {
                const value = e.target.value.replace(/ /g, "");
                this.props.updateFilenameFunc(this.props.id, value);
              }}
              value={this.props.filename}
              title={this.props.language}
            />
            {!this.props.isSingle && (
              <div className="input-group-append">
                <button
                  type="button"
                  className="btn btn-outline-danger"
                  onClick={this.handleRemoveFile}
                >
                  <TrashIcon size={16} />
                </button>
              </div>
            )}
          </div>
        </div>
        <div className={`card-body ${styles.editorContainer}`}>
          <MonacoEditor
            height="400"
            theme="vs-light"
            onChange={(newValue, e) => {
              this.props.updateCodeFunc(this.props.id, newValue);
            }}
            value={this.props.code}
          />
        </div>
      </div>
    );
  }
}

export default CodeEditor;
