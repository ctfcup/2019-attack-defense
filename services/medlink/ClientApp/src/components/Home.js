import React, { Component } from 'react';
import {Telemetry} from "./Telemetry";

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Telemetry/>
        );
    }
}
