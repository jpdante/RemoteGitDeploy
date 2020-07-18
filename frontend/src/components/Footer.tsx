import React from "react";
import { Link } from "@reach/router";

function Footer() {
  return (
    <div>
      <hr />
      <div className="d-flex justify-content-between flex-items-center flex-wrap">
        <a
          className="flex-fill text-center"
          href="https://github.com/jpdante/RemoteGitDeploy/blob/master/LICENSE"
        >
          MIT License
        </a>
        <span className="flex-fill text-center">
          <a
            className="flex-fill text-center"
            href="https://github.com/jpdante"
          >
            João Pedro Dante
          </a>
        </span>
      </div>
      <div className="d-flex justify-content-between flex-items-center flex-wrap">
        <a
          className="flex-fill text-center"
          href="https://github.com/jpdante/RemoteGitDeploy"
        >
          &copy; 2020 RGD
        </a>
      </div>
    </div>
  );
}

export default Footer;
