import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import {BodyModel} from "./components/BodyModel";
import {ModelsList} from "./components/ModelsList";
import './custom.css'
import {ModelInfo} from "./components/ModelInfo";
import Cookies from "js-cookie";
import {Logout} from "./components/Logout";
import {Container} from "reactstrap";

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/upload' component={BodyModel} />
        <Route path='/series' component={ModelsList} />
        <Route path='/info' component={ModelInfo} />
      </Layout>
    );
  }
}
