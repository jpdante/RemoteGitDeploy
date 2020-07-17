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

interface IRepository {
  id: string;
  guid: string;
  name: string;
  description: string;
  team: ITeam;
}

interface IState {
  error: string;
  loading: boolean;
  repositories: IRepository[];
}

class ManageRepositories extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      error: "",
      loading: false,
      repositories: [],
    };
  }

  async componentWillMount() {
    const response = await net.get("/api/get/repositories");
    if (response.data.success) {
      this.setState({
        repositories: response.data.repositories,
      });
    } else {
      this.setState({
        repositories: [],
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
                  <h2 className="text-center mt-3">Managing repositories</h2>
                  <hr />
                  <div className="list-group">
                    {this.state.repositories.map((repository) => (
                      <Link
                        type="button"
                        className="list-group-item list-group-item-action"
                        to={`/repository/${repository.guid}`}
                        key={repository.id}
                      >
                        {repository.team.name}/{repository.name}
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

export default ManageRepositories;
