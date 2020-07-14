import React from "react";
import { Link } from "@reach/router";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";
import net from "../../services/net";

interface IProps {}

interface ITeam {
  id: string;
  name: string;
  description: string;
}

interface IState {
  error: string;
  loading: boolean;
  teams: ITeam[];
}

class ManageTeams extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      error: "",
      loading: false,
      teams: [],
    };
  }

  async componentDidMount() {
    const response = await net.get("/api/get/teams");
    if (response.data.success) {
      this.setState({
        teams: response.data.teams,
      });
    } else {
      this.setState({
        teams: [],
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
                  <h2 className="text-center mt-3">Managing teams</h2>
                  <hr />
                  <div className="list-group">
                    {this.state.teams.map((team) => (
                      <Link
                        type="button"
                        className="list-group-item list-group-item-action"
                        to={`/teams/${team.name}`}
                      >
                        {team.name}
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

export default ManageTeams;
