import React from "react";
import { Router, navigate } from "@reach/router";
import Store from "../undux";
import net from "../services/net";

import Login from "../pages/Login";
import NotFound from "../pages/NotFound";
import Dashboard from "../pages/Dashboard";

import NewSnippet from "../pages/New/NewSnippet";
import NewTeam from "./../pages/New/NewTeam";
import NewUser from "./../pages/New/NewUser";
import NewRepository from "./../pages/New/NewRepository";

import ManageTeams from "./../pages/Manage/ManageTeams";
import ManageSnippets from './../pages/Manage/ManageSnippets';
import Snippet from './../pages/Snippet/index';
import ManageUsers from './../pages/Manage/ManageUsers';
import ManageRepositories from './../pages/Manage/ManageRepositories';
import Repository from './../pages/Repository/index';

const PrivateRoute = (props) => {
  const stores = Store.useStores();
  if (!stores.auth.get("isLogged")) {
    navigate("/");
    return null;
  }
  return React.createElement(props.component, props);
};

class Routes extends React.Component {
  async componentDidMount() {
    net.get("/api/auth/checksession").then((response) => {
      if (response.data.invalidSession === true) {
        this.props.auth.set("token")(null);
      }
    });
  }

  render() {
    return (
      <Router>
        <Login path="/" />
        <PrivateRoute path="/dashboard" component={Dashboard} />

        <PrivateRoute path="/new/snippet" component={NewSnippet} />
        <PrivateRoute path="/new/team" component={NewTeam} />
        <PrivateRoute path="/new/user" component={NewUser} />
        <PrivateRoute path="/new/repository" component={NewRepository} />

        <PrivateRoute path="/manage/snippets" component={ManageSnippets} />
        <PrivateRoute path="/manage/teams" component={ManageTeams} />
        <PrivateRoute path="/manage/users" component={ManageUsers} />
        <PrivateRoute path="/manage/repositories" component={ManageRepositories} />

        <PrivateRoute path="/snippet/:guid" component={Snippet} />
        <PrivateRoute path="/repository/:guid" component={Repository} />

        <NotFound path="*" />
      </Router>
    );
  }
}

export default Store.withStores(Routes);
