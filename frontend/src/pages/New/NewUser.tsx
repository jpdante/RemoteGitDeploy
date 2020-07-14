import React from "react";
import { navigate } from "@reach/router";
import net from "../../services/net";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";

interface IProps {}

interface IState {
  description: string;
  error: string;
  loading: boolean;
  firstName: string;
  lastName: string;
  username: string;
  email: string;
}

class NewUser extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      description: "",
      error: "",
      loading: false,
      firstName: "",
      lastName: "",
      username: "",
      email: "",
    };
  }

  handleSubmit = async (e: any) => {
    e.preventDefault();
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
                  <h2 className="text-center mt-3">Setting up new user</h2>
                  <hr />
                  <div className="form-group">
                    <label>First name</label>
                    <input
                      type="text"
                      className="form-control"
                      required
                      autoComplete="fname"
                      onChange={(e) => this.setState({ firstName: e.target.value })}
                      value={this.state.firstName}
                    />
                  </div>
                  <div className="form-group">
                    <label>Last name</label>
                    <input
                      type="text"
                      className="form-control"
                      required
                      autoComplete="lname"
                      onChange={(e) => this.setState({ lastName: e.target.value })}
                      value={this.state.lastName}
                    />
                  </div>
                  <div className="form-group">
                    <label>Username</label>
                    <input
                      type="text"
                      className="form-control"
                      required
                      autoComplete="username"
                      onChange={(e) => this.setState({ username: e.target.value })}
                      value={this.state.username}
                    />
                  </div>
                  <div className="form-group">
                    <label>Email</label>
                    <input
                      type="email"
                      className="form-control"
                      required
                      autoComplete="email"
                      onChange={(e) => this.setState({ email: e.target.value })}
                      value={this.state.email}
                    />
                  </div>
                  {/*<div className="form-group">
                    <label>User permissions</label>
                    <div className="d-flex justify-content-between flex-items-center flex-wrap">
                      <div className="form-check form-check-inline">
                        <input
                          className="form-check-input"
                          type="checkbox"
                          value="option1"
                        />
                        <label className="form-check-label">1</label>
                      </div>
                      <div className="form-check form-check-inline">
                        <input
                          className="form-check-input"
                          type="checkbox"
                          value="option2"
                        />
                        <label className="form-check-label">2</label>
                      </div>
                      <div className="form-check form-check-inline">
                        <input
                          className="form-check-input"
                          type="checkbox"
                          value="option3"
                          disabled
                        />
                        <label className="form-check-label">3 (disabled)</label>
                      </div>
                    </div>
    </div>*/}
                  <button
                    type="button"
                    className="btn btn-primary float-right"
                    onClick={this.handleSubmit}
                  >
                    Create user
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

export default NewUser;
