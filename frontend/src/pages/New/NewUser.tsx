import React from "react";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";
import net from "../../services/net";

interface IProps {}

interface IState {
  description: string;
  error: string;
  loading: boolean;
  created: boolean;
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
  permissions: IPermissions;
}

interface IPermissions {
  readRepository: boolean;
  writeRepository: boolean;
  manageRepository: boolean;
  readSnippet: boolean;
  writeSnippet: boolean;
  manageSnippet: boolean;
  readAccount: boolean;
  writeAccount: boolean;
  manageAccount: boolean;
  readTeam: boolean;
  writeTeam: boolean;
  manageTeam: boolean;
}

class NewUser extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      description: "",
      error: "",
      loading: false,
      created: false,
      firstName: "",
      lastName: "",
      username: "",
      email: "",
      password: "",
      permissions: {
        readRepository: false,
        writeRepository: false,
        manageRepository: false,
        readSnippet: false,
        writeSnippet: false,
        manageSnippet: false,
        readAccount: false,
        writeAccount: false,
        manageAccount: false,
        readTeam: false,
        writeTeam: false,
        manageTeam: false,
      },
    };
  }

  handleSubmit = async (e: any) => {
    e.preventDefault();
    this.setState({ error: "", loading: false });
    const { firstName, lastName, email, username, permissions } = this.state;
    if (!firstName) {
      this.setState({ error: "The first name cannot be empty." });
      return;
    }
    if (!lastName) {
      this.setState({ error: "The last name cannot be empty." });
      return;
    }
    if (!email) {
      this.setState({ error: "The email cannot be empty." });
      return;
    }
    if (!username) {
      this.setState({ error: "The username cannot be empty." });
      return;
    }
    this.setState({ loading: true });
    await net
      .post("/api/account/new", {
        firstName,
        lastName,
        email,
        username,
        permissions,
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
            this.setState({
              loading: false,
              password: response.data.password,
              created: true,
            });
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
    return (
      <div>
        <NavBar />
        <div className="wrapper">
          <div className="auto-overflow">
            <div className="container content">
              <div className="row">
                <div className="col-sm-1 col-md-2 col-lg-3"></div>
                {this.state.created ? (
                  <div className="col-sm-10 col-md-8 col-lg-6">
                    <h2 className="text-center text-success mt-3">
                      User created successfully
                    </h2>
                    <hr />
                    <h5 className="text-center">
                      Send the data below to the user.
                    </h5>
                    <div className="input-group mb-3">
                      <div className="input-group-prepend">
                        <span className="input-group-text">Email: </span>
                      </div>
                      <input
                        type="text"
                        className="form-control"
                        value={this.state.email}
                        autoComplete="off"
                      />
                    </div>
                    <div className="input-group mb-3">
                      <div className="input-group-prepend">
                        <span className="input-group-text">Password: </span>
                      </div>
                      <input
                        type="text"
                        className="form-control"
                        value={this.state.password}
                        autoComplete="off"
                      />
                    </div>
                  </div>
                ) : (
                  <div className="col-sm-10 col-md-8 col-lg-6">
                    <h2 className="text-center mt-3">Setting up new user</h2>
                    <hr />
                    {this.state.error && (
                      <div className="alert alert-danger" role="alert">
                        {this.state.error}
                      </div>
                    )}
                    <div className="form-group">
                      <label>First name</label>
                      <input
                        type="text"
                        className="form-control"
                        required
                        autoComplete="fname"
                        onChange={(e) =>
                          this.setState({ firstName: e.target.value })
                        }
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
                        onChange={(e) =>
                          this.setState({ lastName: e.target.value })
                        }
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
                        onChange={(e) =>
                          this.setState({ username: e.target.value })
                        }
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
                        onChange={(e) =>
                          this.setState({ email: e.target.value })
                        }
                        value={this.state.email}
                      />
                    </div>
                    <hr />
                    <div className="form-group">
                      <label>Repository permissions</label>
                      <div className="d-flex justify-content-between flex-items-center flex-wrap">
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.readRepository}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.readRepository = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">View</label>
                        </div>
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.writeRepository}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.writeRepository = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">Create</label>
                        </div>
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.manageRepository}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.manageRepository = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">Manage</label>
                        </div>
                      </div>
                    </div>
                    <hr />
                    <div className="form-group">
                      <label>Snippet permissions</label>
                      <div className="d-flex justify-content-between flex-items-center flex-wrap">
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.readSnippet}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.readSnippet = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">View</label>
                        </div>
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.writeSnippet}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.writeSnippet = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">Create</label>
                        </div>
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.manageSnippet}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.manageSnippet = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">Manage</label>
                        </div>
                      </div>
                    </div>
                    <hr />
                    <div className="form-group">
                      <label>Account permissions</label>
                      <div className="d-flex justify-content-between flex-items-center flex-wrap">
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.readAccount}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.readAccount = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">View</label>
                        </div>
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.writeAccount}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.writeAccount = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">Create</label>
                        </div>
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.manageAccount}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.manageAccount = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">Manage</label>
                        </div>
                      </div>
                    </div>
                    <hr />
                    <div className="form-group">
                      <label>Team permissions</label>
                      <div className="d-flex justify-content-between flex-items-center flex-wrap">
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.readTeam}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.readTeam = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">View</label>
                        </div>
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.writeTeam}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.writeTeam = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">Create</label>
                        </div>
                        <div className="form-check form-check-inline">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            checked={this.state.permissions.manageTeam}
                            onChange={(e) => {
                              let permissions = this.state.permissions;
                              permissions.manageTeam = e.target.checked;
                              this.setState({
                                permissions: permissions,
                              });
                            }}
                          />
                          <label className="form-check-label">Manage</label>
                        </div>
                      </div>
                    </div>
                    <hr />
                    <button
                      type="button"
                      className="btn btn-primary float-right"
                      onClick={this.handleSubmit}
                    >
                      Create user
                    </button>
                  </div>
                )}
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
