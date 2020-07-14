import React from "react";
import { navigate } from "@reach/router";
import net from "../../services/net";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";

interface IProps {}

interface IState {
  error: string;
  loading: boolean;
  name: string;
  description: string;
}

class NewTeam extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      error: "",
      loading: false,
      name: "",
      description: "",
    };
  }

  handleSubmit = async (e: any) => {
    e.preventDefault();
    this.setState({ error: "", loading: false });
    const { name, description } = this.state;
    if (!name) {
      this.setState({ error: "The team name cannot be empty." });
      return;
    }
    this.setState({ loading: true });
    const response = await net.post("/api/new/team", {
      name,
      description,
    });
    if (response.data.success) {
      this.setState({
        loading: false,
        error: "",
      });
      navigate("/team/" + name);
    } else {
      this.setState({
        loading: false,
        error: response.data.message,
      });
    }
  };

  render() {
    return (
      <div>
        <NavBar />
        <div className="wrapper">
          <div className="auto-overflow">
            <div className="container content">
              <div className="row">
                <div className="col-sm-1 col-md-2 col-lg-3"></div>
                <div className="col-sm-10 col-md-8 col-lg-6">
                  <h2 className="text-center mt-3">Setting up new team</h2>
                  <hr />
                  {this.state.error && (
                    <div className="alert alert-danger" role="alert">
                      {this.state.error}
                    </div>
                  )}
                  <div className="form-group">
                    <label>Team name</label>
                    <input
                      type="text"
                      className="form-control"
                      onChange={(e) => {
                        const value = e.target.value.replace(/ /g, "");
                        this.setState({ name: value });
                      }}
                      value={this.state.name}
                    />
                  </div>
                  <div className="form-group">
                    <label>Team description</label>
                    <input
                      type="text"
                      className="form-control"
                      onChange={(e) => {
                        this.setState({ description: e.target.value });
                      }}
                      value={this.state.description}
                    />
                  </div>
                  <button
                    type="button"
                    className="btn btn-primary float-right"
                    onClick={this.handleSubmit}
                  >
                    {this.state.loading ? (
                      <div className="spinner-border" role="status">
                        <span className="sr-only">Loading...</span>
                      </div>
                    ) : (
                      "Create team"
                    )}
                  </button>
                </div>
                <div className="col-sm-1 col-md-2 col-lg-3"></div>
              </div>
              <Footer />
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default NewTeam;
