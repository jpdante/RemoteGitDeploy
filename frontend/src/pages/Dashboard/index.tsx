import React from "react";
import NavBar from "../../components/NavBar";
import SideBar from "../../components/SideBar";
import Footer from "../../components/Footer";
import net from "../../services/net";
import RepositoryHistory from "../../components/RepositoryHistory";

interface IHistoryParameter {
  name: string;
  value: string;
  color: string;
}

interface IHistory {
  id: string;
  repository: string;
  icon: number;
  name: string;
  date: string;
  parameters: IHistoryParameter[];
}

interface IState {
  history: IHistory[];
}

interface IProps {}
class Dashboard extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      history: [],
    };
  }

  async componentDidMount() {
    const response = await net.get("/api/get/repositories/history");
    if (response.data.success) {
      this.setState({
        history: response.data.history,
      });
    } else {
      this.setState({
        history: [],
      });
    }
  }

  render() {
    return (
      <div>
        <NavBar />
        <div className="container-fluid wrapper">
          <div className="row">
            <SideBar />
            <div className="col-12 col-md-8 col-lg-9 content">
              {this.state.history.map((history) => (
                <RepositoryHistory history={history} />
              ))}
              <Footer />
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default Dashboard;
