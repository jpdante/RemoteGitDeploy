import React from "react";
import { Link } from "@reach/router";
import type { StoreProps } from "../../undux";
import Store from "../../undux";
import UserBar from "./UserBar";
import NavLink from "./NavLink";

class NavBar extends React.Component<StoreProps> {
  render() {
    const { auth } = this.props;
    return (
      <nav className="navbar navbar-expand-md navbar-dark bg-darker fixed-top">
        <Link to="/dashboard" className="navbar-brand hidden-md">
          RGD
        </Link>
        <button
          className="navbar-toggler"
          type="button"
          data-toggle="collapse"
          data-target="#navbarNav"
          aria-controls="navbarNav"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarNav">
          <div className="d-flex w-100 mr-auto navbar-margin-top-md">
            <ul className="navbar-nav w-100">
              <NavLink to="/dashboard">Dashboard</NavLink>
            </ul>
          </div>
          {auth.get("isLogged") && <UserBar />}
        </div>
      </nav>
    );
  }
}

export default Store.withStores(NavBar);
