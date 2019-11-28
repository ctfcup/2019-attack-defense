import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import {Logout} from "./Logout";
import {Login} from "./Login";
import Cookies from "js-cookie";

export class Layout extends Component {
    static displayName = Layout.name;

    constructor(props) {
        super(props);
        this.state = {
            loggedIn: Cookies.get("medlinkToken") != null
        };
        this.onSignIn = this.onSignIn.bind(this)
    }

    onSignIn() {
        this.setState({
            loggedIn: Cookies.get("medlinkToken") != null
        });
    }

    render() {
        return (
            <div>
                <NavMenu loggedIn={this.state.loggedIn}/>
                <Container>
                    {!this.state.loggedIn && <Login onSignIn={this.onSignIn}/>}
                    {this.state.loggedIn && this.props.children}
                    <br/>
                    <br/>
                    {this.state.loggedIn && <Logout onSignIn={this.onSignIn}/>}
                </Container>
            </div>
        );
    }
}
