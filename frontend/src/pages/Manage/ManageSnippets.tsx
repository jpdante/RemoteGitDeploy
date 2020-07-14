import React from "react";
import { Link } from "@reach/router";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";
import net from "../../services/net";

interface IProps {}

interface IFile {
  filename: string;
  code: string;
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
  snippets: ISnippet[];
}

class ManageSnippets extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      error: "",
      loading: false,
      snippets: [],
    };
  }

  async componentDidMount() {
    const response = await net.get("/api/get/snippets");
    if (response.data.success) {
      this.setState({
        snippets: response.data.snippets,
      });
    } else {
      this.setState({
        snippets: [],
      });
    }
  }

  render() {
    return (
      <div>
        <NavBar />
        <div className="wrapper">
          <div className="auto-overflow">
            <div className="container content">
              <div className="row">
                <div className="col-sm-1 col-md-2 col-lg-2"></div>
                <div className="col-sm-10 col-md-8 col-lg-8">
                  <h2 className="text-center mt-3">Managing snippets</h2>
                  <hr />
                  <div className="list-group">
                    {this.state.snippets.map((snippet) => (
                      <Link
                        type="button"
                        className="list-group-item d-flex align-items-center"
                        to={`/snippet/${snippet.guid}`}
                        key={snippet.id}
                      >
                        <span className="mr-auto">{snippet.description}</span>
                        {snippet.files.map((file) => (
                          <span className="badge badge-primary mx-1">
                            {file.filename}
                          </span>
                        ))}
                      </Link>
                    ))}
                  </div>
                </div>
                <div className="col-sm-1 col-md-2 col-lg-2"></div>
              </div>
              <Footer />
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default ManageSnippets;
