import React from "react";

import "./libraries/fontawesome";

import Store from "./undux";
import Routes from "./routes";

class App extends React.Component {
  render() {
    return (
      <Store.Container>
        <Routes />
      </Store.Container>
    );
  }
}

export default App;
