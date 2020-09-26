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
  content: string;
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
    await net
      .post("/api/snippet/get", {
        guid,
      })
      .then((response) => {
        if (response.data) {
          if (response.data.error) {
            navigate("/");
            return;
          }
          if (response.data.success) {
            this.setState({
              snippet: response.data.snippet,
            });
          }
        }
      })
      .catch((reason) => {
        if (
          reason.response &&
          reason.response.data &&
          reason.response.data.error
        ) {
          navigate("/");
        }
      });
  }

  render() {
    return (
      <div>
        <NavBar />
        <div className="wrapper">
          <div className="auto-overflow">
            <div className="container content">
              {this.state.snippet.files.map((file, index) => (
                <div className={`card ${styles.snippet}`} key={index}>
                  <div className="card-header">{file.filename}</div>
                  <div className={`card-body`}>
                    <Highlight language={file.language}>{file.content}</Highlight>
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
