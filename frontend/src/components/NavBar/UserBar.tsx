/* eslint-disable jsx-a11y/anchor-is-valid */
import React from "react";
import Store from "../../undux";
import { GearIcon, BellIcon, PlusIcon, TriangleDownIcon } from "@primer/octicons-react";
import { Link } from "@reach/router";
import net from "../../services/net";

function UserBar() {
  const { auth, profile } = Store.useStores();

  async function logout() {
    await net.get("/auth/logout");
    auth.set("token")(null);
  }

  return (
    <ul className="nav navbar-nav ml-auto w-100 justify-content-end">
      <li className="nav-item dropdown">
        <a
          className="nav-link text-white"
          href="#"
          data-toggle="dropdown"
          aria-haspopup="true"
          aria-expanded="false"
        >
          <BellIcon size={16} />
          <TriangleDownIcon size={16} />
        </a>
        <div
          className={`dropdown-menu dropdown-menu-right`}
          aria-labelledby="navbarDropdownMenuLink"
        >
          <label className="dropdown-item">No Notifications :D</label>
        </div>
      </li>
      <li className="nav-item dropdown">
        <a
          className="nav-link text-white"
          href="#"
          data-toggle="dropdown"
          aria-haspopup="true"
          aria-expanded="false"
        >
          <GearIcon size={16} />
          <TriangleDownIcon size={16} />
        </a>
        <div
          className={`dropdown-menu dropdown-menu-right`}
          aria-labelledby="navbarDropdownMenuLink"
        >
          <Link className="dropdown-item" to="/manage/repositories">Manage repositories</Link>
          <Link className="dropdown-item" to="/manage/teams">Manage teams</Link>
          <Link className="dropdown-item" to="/manage/snippets">Manage snippets</Link>
          <Link className="dropdown-item" to="/manage/users">Manage users</Link>
        </div>
      </li>
      <li className="nav-item dropdown">
        <a
          className="nav-link text-white"
          href="#"
          data-toggle="dropdown"
          aria-haspopup="true"
          aria-expanded="false"
        >
          <PlusIcon size={16} />
          <TriangleDownIcon size={16} />
        </a>
        <div
          className={`dropdown-menu dropdown-menu-right`}
          aria-labelledby="navbarDropdownMenuLink"
        >
          <Link className="dropdown-item" to="/new/repository">New repository</Link>
          <Link className="dropdown-item" to="/new/team">New team</Link>
          <Link className="dropdown-item" to="/new/snippet">New snippet</Link>
          <Link className="dropdown-item" to="/new/user">New user</Link>
        </div>
      </li>
      <li className="nav-item dropdown">
        <a
          className="nav-link text-white"
          href="#"
          data-toggle="dropdown"
          aria-haspopup="true"
          aria-expanded="false"
        >
          {profile.get("username")}
          <TriangleDownIcon size={16} />
        </a>
        <div
          className={`dropdown-menu dropdown-menu-right`}
          aria-labelledby="navbarDropdownMenuLink"
        >
          <button className="dropdown-item" onClick={logout}>
            Logout
          </button>
        </div>
      </li>
    </ul>
  );
}

export default UserBar;
