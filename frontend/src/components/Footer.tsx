import React from "react";
import { Link } from "@reach/router";

function Footer() {
  return (
    <div>
      <hr />
      <div className="d-flex justify-content-between flex-items-center flex-wrap">
        <Link className="flex-fill text-center" to="/dashboard">© 2020 GRD</Link>
        <h6 className="flex-fill text-center">penis</h6>
      </div>
    </div>
  );
}

export default Footer;
