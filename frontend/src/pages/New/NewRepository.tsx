import React from "react";
import net from "../../services/net";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";
import Loading from "../../components/Loading";

interface IProps {}

interface ITeam {
  id: string;
  name: string;
  description: string;
}

interface IState {
  description: string;
  error: string;
  loadingTeams: boolean;
  teams: ITeam[];
}

class NewRepository extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      description: "",
      error: "",
      loadingTeams: false,
      teams: [],
    };
  }

  async componentDidMount() {
    this.setState({ loadingTeams: true });
    const response = await net.get("/api/get/teams");
    if (response.data.success) {
      this.setState({
        loadingTeams: false,
        teams: response.data.teams,
      });
    } else {
      this.setState({ loadingTeams: false });
    }
  }

  handleSubmit = async (e: any) => {
    e.preventDefault();
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
                  <div className="form-group">
                    <label>Github SSH Git</label>
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Ex: git@github.com:jpdante/HtcSharp.git"
                    />
                  </div>
                  <div className="form-group">
                    <label>Repository description</label>
                    <input
                      type="text"
                      className="form-control"
                      placeholder="ThisIsMyRepo"
                    />
                  </div>
                  <div className="form-group">
                    <label>Team</label>
                    <select className="form-control">
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
                    Create repository
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
