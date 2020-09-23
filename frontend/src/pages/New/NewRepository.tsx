import React from "react";
import net from "../../services/net";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";
import Loading from "../../components/Loading";
import { navigate } from "@reach/router";
import ConsoleLog from "../../components/ConsoleLog";

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
  requestSent: boolean;
  statusGuid: string;
  git: string;
  name: string;
  team: string;
  description: string;
  status: IStatus;
  intervalId: any;
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
      requestSent: false,
      git: "",
      name: "",
      team: "",
      description: "",
      statusGuid: "",
      status: {
        success: false,
        finished: false,
        log: [],
        guid: "",
      },
      intervalId: null,
    };
  }

  async componentWillMount() {
    this.setState({ loadingTeams: true });
    await net
      .get("/api/team/get")
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

  componentDidMount() {
    var intervalId = setInterval(this.timerElapsed, 1000);
    this.setState({ intervalId: intervalId });
  }

  componentWillUnmount() {
    clearInterval(this.state.intervalId);
  }

  reloadBranchs = async (git: string) => {
    if (!git) {
      this.setState({ error: "The git URL cannot be empty." });
      return;
    }
    const response = await net.post("/api/get/repositorybranchlist", {
      git,
    });
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
    } else {
      this.setState({
        branchs: [],
        branch: "",
        error:
          "Failed to get branches from the git repository, is it a valid git url ?",
      });
    }
  };

  timerElapsed = async () => {
    if (!this.state.requestSent) return;
    const { statusGuid } = this.state;
    const response = await net.post("/api/status/newrepository", {
      guid: statusGuid,
    });
    if (response.data.success) {
      this.setState({
        status: response.data.status,
      });
      if (this.state.status.finished) {
        clearInterval(this.state.intervalId);
        if (this.state.status.success) {
          const { guid } = this.state.status;
          setTimeout(function () {
            navigate("/repository/" + guid);
          }, 3000);
        }
      }
    }
  };

  handleSubmit = async (e: any) => {
    e.preventDefault();
    this.setState({ error: "", loading: false });
    const { git, name, branch, description, team } = this.state;
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
    const response = await net.post("/api/new/repository", {
      git,
      branch,
      name,
      description,
      team,
    });
    if (response.data.success) {
      this.setState({
        loading: false,
        error: "",
        requestSent: true,
        statusGuid: response.data.status,
      });
    } else {
      this.setState({
        loading: false,
        error: response.data.message,
        requestSent: false,
      });
    }
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
              {this.state.requestSent ? (
                <div>
                  {this.state.status.finished ? (
                    this.state.status.success ? (
                      <div>
                        <h2 className="text-center mt-3 text-success">
                          Repository successfully configured!
                        </h2>
                        <h4 className="text-center my-3">Redirecting...</h4>
                      </div>
                    ) : (
                      <div>
                        <h2 className="text-center mt-3 text-danger">
                          Failed to setup repository '{this.state.name}'
                        </h2>
                      </div>
                    )
                  ) : (
                    <div>
                      <h2 className="text-center mt-3">
                        Setting up '{this.state.name}' repository
                      </h2>
                      <h4 className="text-center my-3">Please wait...</h4>
                    </div>
                  )}
                  <ConsoleLog log={this.state.status.log} insideCard={false} />
                </div>
              ) : (
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
                      <label>SSH Git Link</label>
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
                        onChange={(e) =>
                          this.setState({ team: e.target.value })
                        }
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
              )}
              <Footer />
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default NewRepository;
