import React from "react";
import { Link } from "@reach/router";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";
import net from "../../services/net";

interface IProps {}

interface IUser {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  username: string;
}

interface IState {
  error: string;
  loading: boolean;
  users: IUser[];
}

class ManageUsers extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      error: "",
      loading: false,
      users: [],
    };
  }

  async componentWillMount() {
    const response = await net.get("/api/get/users");
    if (response.data.success) {
      this.setState({
        users: response.data.users,
      });
    } else {
      this.setState({
        users: [],
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
                  <h2 className="text-center mt-3">Managing users</h2>
                  <hr />
                  <div className="list-group">
                    {this.state.users.map((user) => (
                      <Link
                        type="button"
                        className="list-group-item d-flex align-items-center list-group-item-action"
                        to={`/user/${user.username}`}
                        key={user.id}
                      >
                        <span className="mr-auto">
                          {user.firstName} {user.lastName}
                        </span>
                        <span className="badge badge-info mx-1">
                          {user.username}
                        </span>
                        <span className="badge badge-primary mx-1">
                          {user.email}
                        </span>
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

export default ManageUsers;
