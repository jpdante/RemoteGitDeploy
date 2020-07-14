import React from "react";
import { navigate } from "@reach/router";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";
import net from "../../services/net";

import Highlight from "react-highlight.js";

import styles from "./snippet.module.scss";

interface IProps {
  guid: string;
}

interface IFile {
  filename: string;
  code: string;
  language: string;
}

interface ISnippet {
  id: string;
  guid: string;
  description: string;
  files: IFile[];
}

interface IState {
  error: string;
  loading: boolean;
  snippet: ISnippet;
}

class Snippet extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      error: "",
      loading: false,
      snippet: {
        id: "",
        guid: "",
        description: "",
        files: [],
      },
    };
  }

  async componentDidMount() {
    const { guid } = this.props;
    const response = await net.post("/api/get/snippet", {
      guid,
    });
    if (response.data.success) {
      this.setState({
        snippet: response.data.snippet,
      });
    } else {
      navigate("/");
    }
  }

  render() {
    return (
      <div>
        <NavBar />
        <div className="wrapper">
          <div className="auto-overflow">
            <div className="container content">
              {this.state.snippet.files.map((file) => (
                <div className={`card ${styles.snippet}`}>
                  <div className="card-header">{file.filename}</div>
                  <div className={`card-body`}>
                    <Highlight language={file.language}>{file.code}</Highlight>
                  </div>
                </div>
              ))}
              <Footer />
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default Snippet;
