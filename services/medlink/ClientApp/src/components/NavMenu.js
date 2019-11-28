import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import {isLoggedIn} from "./utils";

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar() {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render() {
    return (
        <header>
          <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
            <Container>
              <NavbarBrand tag={Link} to="/">Medlink</NavbarBrand>
              <NavbarToggler onClick={this.toggleNavbar} className="mr-2"/>
              <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                <ul className="navbar-nav flex-grow">
                  {!isLoggedIn() &&
                  <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/">SignIn</NavLink>
                  </NavItem>
                  }
                  {isLoggedIn() &&
                  <>
                    <NavItem>
                      <NavLink tag={Link} className="text-dark" to="/">Check</NavLink>
                    </NavItem>
                    <NavItem>
                      <NavLink tag={Link} className="text-dark" to="/upload">Upload</NavLink>
                    </NavItem>
                    <NavItem>
                      <NavLink tag={Link} className="text-dark" to="/info">Info</NavLink>
                    </NavItem>
                  </>
                  }
                  <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/series">Supported Series</NavLink>
                  </NavItem>
                </ul>
              </Collapse>
            </Container>
          </Navbar>
        </header>
    );
  }
}
