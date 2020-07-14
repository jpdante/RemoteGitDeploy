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
    };
  }

  handleSubmit = async (e: any) => {
    e.preventDefault();
    this.setState({ error: "", loading: false });
    const { firstName, lastName, email, username } = this.state;
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
    const response = await net.post("/api/new/user", {
      firstName,
      lastName,
      email,
      username,
    });
    if (response.data.success) {
      this.setState({
        loading: false,
        password: response.data.password,
        created: true,
      });
    } else {
      this.setState({
        loading: false,
        error: response.data.message,
      });
    }
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
