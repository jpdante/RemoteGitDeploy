import React, { ReactNode } from "react";
import { Link } from "@reach/router";

type PropType = {
  to: string;
  children?: ReactNode;
};

function NavLink(props: PropType) {
  return (
    <li className="nav-item">
      <Link
        {...props}
        getProps={({ isCurrent }) => {
          return {
            className: isCurrent ? "nav-link active" : "nav-link",
          };
        }}
      >
        {props.children}
        <div className="nav-item-active" />
      </Link>
    </li>
  );
}

export default NavLink;
