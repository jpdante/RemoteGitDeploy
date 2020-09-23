import React from "react";
import { navigate, Redirect } from "@reach/router";
import type { StoreProps } from "../../undux";
import Store from "../../undux";

import styles from "./login.module.scss";
import net from "../../services/net";

interface IState {
  username: string;
  password: string;
  rememberMe: boolean;
  error: string;
  loading: boolean;
}

class Login extends React.Component<StoreProps, IState> {
  constructor(props: StoreProps) {
    super(props);
    this.state = {
      username: "",
      password: "",
      rememberMe: false,
      error: "",
      loading: false,
    };
  }

  handleSubmit = async (e: any) => {
    e.preventDefault();
    this.setState({
      error: "",
      loading: false,
    });
    const { username, password, rememberMe } = this.state;
    if (!username) {
      this.setState({ error: "errors.emailEmpty" });
      return;
    }
    if (!password) {
      this.setState({ error: "errors.passwordEmpty" });
      return;
    }
    this.setState({ loading: true });
    await net
      .post("/api/auth/login", {
        username,
        password,
        rememberMe,
      })
      .then((response) => {
        console.error(response);
        if (!response.data) return;
        if (response.data.error) {
          this.setState({
            error: response.data.error.message,
          });
        } else {
          const { auth, profile } = this.props;
          profile.set("username")(response.data.account.username);
          profile.set("email")(response.data.account.email);
          auth.set("token")(response.data.token);
          navigate("/dashboard");
        }
      })
      .catch((reason) => {
        if (
          reason.response &&
          reason.response.data &&
          reason.response.data.error
        ) {
          this.setState({
            error: reason.response.data.error.message,
          });
        }
      });
  };

  render() {
    const { auth } = this.props;
    if (auth.get("isLogged")) {
      return <Redirect to="/dashboard" noThrow />;
    }
    return (
      <div className={styles.container}>
        <form className={styles.formSignIn} onSubmit={this.handleSubmit}>
          <img
            className="mb-4"
            src="/logo512.png"
            alt=""
            width="72"
            height="72"
          />
          <h1 className="h3 mb-3 font-weight-normal">Please sign in</h1>
          {this.state.error && (
            <div className="alert alert-danger" role="alert">
              {this.state.error}
            </div>
          )}
          <label htmlFor="inputEmail" className="sr-only">
            Username
          </label>
          <input
            type="text"
            id="inputEmail"
            className={`form-control ${styles.formControl}`}
            placeholder="Email address"
            required
            autoFocus
            autoComplete="email"
            onChange={(e) => this.setState({ username: e.target.value })}
            value={this.state.username}
          />
          <label htmlFor="inputPassword" className="sr-only">
            Password
          </label>
          <input
            type="password"
            id="inputPassword"
            className={`form-control ${styles.formControl}`}
            placeholder="Password"
            required
            autoComplete="password"
            onChange={(e) => this.setState({ password: e.target.value })}
            value={this.state.password}
          />
          <div className={`${styles.checkbox} mb-3`}>
            <label>
              <input
                type="checkbox"
                value="remember-me"
                onChange={(e) =>
                  this.setState({ rememberMe: e.target.checked })
                }
                checked={this.state.rememberMe}
              />{" "}
              Remember me
            </label>
          </div>
          <button className="btn btn-lg btn-primary btn-block" type="submit">
            Sign in
          </button>
          <p className="mt-5 mb-3 text-muted">&copy; 2020</p>
        </form>
      </div>
    );
  }
}

export default Store.withStores(Login);
