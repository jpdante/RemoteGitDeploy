import React from "react";

class NotFound extends React.Component {
  render() {
    return (
      <form className="form-signin">
        <img
          className="mb-4"
          src="/docs/4.5/assets/brand/bootstrap-solid.svg"
          alt=""
          width="72"
          height="72"
        />
        <h1 className="h3 mb-3 font-weight-normal">404 - Page Not Found</h1>
      </form>
    );
  }
}

export default NotFound;
