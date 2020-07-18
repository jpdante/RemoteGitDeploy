import React from "react";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";
import RepositoryBar from "./../../components/RepositoryBar/index";
import net from "../../services/net";
import { navigate } from "@reach/router";
import Loading from "../../components/Loading";
import StatusTab from "./status";
import ActionsTab from "./actions";
import SettingsTab from "./settings";

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

interface IProps {
  guid: string;
}

interface IState {
  loading: boolean;
  repository: IRepository;
}

class Repository extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      loading: false,
      repository: {
        id: "",
        guid: "",
        name: "",
        description: "",
        team: {
          id: "",
          name: "",
          description: "",
        },
      },
    };
  }

  async componentDidMount() {
    const { guid } = this.props;
    this.setState({
      loading: true,
    });
    const response = await net.post("/api/get/repository", {
      guid,
    });
    if (response.data.success) {
      this.setState({
        loading: false,
        repository: response.data.repository,
      });
    } else {
      navigate("/dashboard");
    }
  }

  render() {
    if (this.state.loading) {
      return <Loading />;
    }
    return (
      <div>
        <NavBar />
        <div className="wrapper">
          <div className="auto-overflow">
            <div className="content-nopadding bg-white">
              <RepositoryBar repository={this.state.repository} />
              <div className="container">
                <div className="tab-content">
                  <StatusTab repository={this.state.repository} />
                  <ActionsTab repository={this.state.repository} />
                  <SettingsTab repository={this.state.repository} />
                </div>
                <Footer />
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default Repository;
