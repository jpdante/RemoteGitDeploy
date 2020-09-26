import React from "react";
import net from "../../services/net";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";
import Loading from "../../components/Loading";
import { navigate } from "@reach/router";

interface IProps {}

interface ITeam {
  id: string;
  name: string;
  description: string;
}

interface ILog {
  data: string;
  timespan: number;
}

interface IStatus {
  success: boolean;
  finished: boolean;
  log: ILog[];
  guid: string;
}

interface IState {
  error: string;
  loadingTeams: boolean;
  loading: boolean;
  teams: ITeam[];
  branchs: string[];
  branch: string;
  git: string;
  username: string;
  token: string;
  name: string;
  team: string;
  description: string;
}

class NewRepository extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      error: "",
      loadingTeams: false,
      loading: false,
      teams: [],
      branchs: [],
      branch: "",
      username: "",
      token: "",
      git: "",
      name: "",
      team: "",
      description: "",
    };
  }

  async componentWillMount() {
    this.setState({ loadingTeams: true });
    await net
      .get("/api/teams/get")
      .then((response) => {
        if (response.data) {
          if (response.data.error) {
            this.setState({
              loading: false,
              loadingTeams: false,
              error: response.data.error.message,
            });
            return;
          }
          if (response.data.success) {
            if (response.data.teams.length > 0) {
              this.setState({
                loading: false,
                loadingTeams: false,
                teams: response.data.teams,
                team: response.data.teams[0].name,
              });
            } else {
              this.setState({
                loading: false,
                loadingTeams: false,
                error:
                  "No teams were found, please create a team before creating a repository.",
              });
            }
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
  }

  reloadBranchs = async (git: string) => {
    let { username, token } = this.state;
    if (!username) {
      this.setState({ error: "The username cannot be empty." });
      return;
    }
    if (!token) {
      this.setState({ error: "The private access token cannot be empty." });
      return;
    }
    if (!git) {
      this.setState({ error: "The git URL cannot be empty." });
      return;
    }
    await net
      .post("/api/repository/branchs/get", {
        git,
        username,
        token
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
            if (response.data.branchs.length > 0) {
              this.setState({
                branchs: response.data.branchs,
                branch: response.data.branchs[0],
                error: "",
              });
            } else {
              this.setState({
                branchs: response.data.branchs,
                error: "",
              });
            }
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

  handleSubmit = async (e: any) => {
    e.preventDefault();
    this.setState({ error: "", loading: false });
    const { username, token, git, name, branch, description, team } = this.state;
    if (!username) {
      this.setState({ error: "The username cannot be empty." });
      return;
    }
    if (!token) {
      this.setState({ error: "The private access token cannot be empty." });
      return;
    }
    if (!git) {
      this.setState({ error: "The git URL cannot be empty." });
      return;
    }
    if (!branch) {
      this.setState({ error: "The git branch cannot be empty." });
      return;
    }
    if (!name) {
      this.setState({ error: "The repository name cannot be empty." });
      return;
    }
    if (!team) {
      this.setState({ error: "The team cannot be empty." });
      return;
    }
    this.setState({ loading: true });
    await net
      .post("/api/repository/new", {
        username,
        token,
        git,
        branch,
        name,
        description,
        team,
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
            navigate("/repository/" + response.data.guid);
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

  render() {
    if (this.state.loadingTeams) {
      return <Loading />;
    }
    return (
      <div>
        <NavBar />
        <div className="wrapper">
          <div className="auto-overflow">
            <div className="container content">
              <div className="row">
                <div className="col-sm-1 col-md-2 col-lg-3"></div>
                <div className="col-sm-10 col-md-8 col-lg-6">
                  <h2 className="text-center mt-3">
                    Setting up new repository
                  </h2>
                  <hr />
                  {this.state.error && (
                    <div className="alert alert-danger" role="alert">
                      {this.state.error}
                    </div>
                  )}
                  <div className="form-group">
                    <label>Git Username</label>
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Ex: cleber"
                      onChange={(e) => {
                        this.setState({ username: e.target.value });
                      }}
                      value={this.state.username}
                    />
                  </div>
                  <div className="form-group">
                    <label>Git Personal Access Token</label>
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Ex: exoy73va946u3xflnd7xgpt5o2ishm1lxzejs4iz"
                      onChange={(e) => {
                        this.setState({ token: e.target.value });
                      }}
                      value={this.state.token}
                    />
                  </div>
                  <div className="form-group">
                    <label>Git Link</label>
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Ex: https://github.com/jpdante/HtcSharp.git"
                      onChange={(e) => {
                        this.setState({ git: e.target.value });
                        this.reloadBranchs(e.target.value);
                      }}
                      value={this.state.git}
                    />
                  </div>
                  <div className="form-group">
                    <label>Branch</label>
                    <select
                      className="form-control"
                      onChange={(e) =>
                        this.setState({ branch: e.target.value })
                      }
                      value={this.state.branch}
                    >
                      {this.state.branchs.map((branch, index) => (
                        <option key={index}>{branch}</option>
                      ))}
                    </select>
                  </div>
                  <div className="form-group">
                    <label>Repository name</label>
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Ex: HtcSharp"
                      onChange={(e) => {
                        const value = e.target.value.replace(/ /g, "");
                        this.setState({ name: value });
                      }}
                      value={this.state.name}
                    />
                  </div>
                  <div className="form-group">
                    <label>Repository description</label>
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Ex: This is my repository..."
                      onChange={(e) =>
                        this.setState({ description: e.target.value })
                      }
                      value={this.state.description}
                    />
                  </div>
                  <div className="form-group">
                    <label>Team</label>
                    <select
                      className="form-control"
                      onChange={(e) => this.setState({ team: e.target.value })}
                      value={this.state.team}
                    >
                      {this.state.teams.map((team) => (
                        <option key={team.id}>{team.name}</option>
                      ))}
                    </select>
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
                      "Create repository"
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

export default NewRepository;
