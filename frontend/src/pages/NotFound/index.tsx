import { navigate } from "@reach/router";
import React from "react";

class NotFound extends React.Component {
  componentDidMount() {
    setTimeout(() => navigate("/dashboard", { replace: true }), 0);
  }

  render() {
    return null;
  }
}

export default NotFound;
