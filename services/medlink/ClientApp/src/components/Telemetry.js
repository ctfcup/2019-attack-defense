import React, { Component } from 'react';
import { Alert, Button, Col, Form, FormGroup, Input, Label } from 'reactstrap';

export class Telemetry extends Component {
    constructor(props) {
        super(props);
        this.state = {
            modelSeries: "",
            revision: "",
            bodyID: "",
            paramsValue: {},
            report: [],
            loading: true,
            error: null,
        };

        this.handleChange = this.handleChange.bind(this);
        this.submitTelemetryForm = this.submitTelemetryForm.bind(this);
        this.submitModelSeriesForm = this.submitModelSeriesForm.bind(this);
    }

    componentDidMount() {
        this.populateHealthCheckData();
    }

    submitTelemetryForm(e) {
        e.preventDefault();
        this.setState({error: null});
        fetch('api/telemetry', {
            method: 'PUT',
            body: JSON.stringify({
                owner: localStorage.getItem("login"),
                bodymodelseries: this.state.modelSeries,
                bodyRevision: this.state.revision,
                bodyID: this.state.bodyID,
                hardwaretelemetry: {...this.state.paramsValue}
            })
        }).then(resp => {
            if (!resp.ok)
                throw resp;
            this.populateHealthCheckData();
        }).catch(_ => this.setState({error: "Unknown error"}));
    };

    submitModelSeriesForm(e) {
        e.preventDefault();
        this.setState({error: null});
        fetch(`api/template?modelSeries=${this.state.modelSeries}&revision=${this.state.revision}`, {
            method: 'GET'
        }).then(resp => {
            if (!resp.ok)
                throw resp;
            resp.json().then(content => {
                const paramsValue = {};
                for (var i = 0; i < content.length; i++) {
                    paramsValue[content[i]] = 0.0;
                }
                this.setState({paramsValue});
            })
        }).catch(_ => {
            this.setState({error: "Unknown error"})
        });
    };

    handleChange = async (event) => {
        const {target} = event;
        const {name} = target;
        await this.setState({
            [name]: target.value,
        });
    };

    HandleRefValueChange = async (event) => {
        const {target} = event;
        const {name} = target;

        const paramsValue = {...this.state.paramsValue};
        paramsValue[name] = target.value;
        this.setState({paramsValue});
    };

    render() {
        return (
            <div>
                {this.renderHealthCheck()}
                {this.renderBodySeriesForm()}
                {Object.keys(this.state.paramsValue).length > 0 ? this.renderSendSTelemetryForm() : ""}
            </div>
        )
    }

    renderBodySeriesForm() {
        const {modelSeries, revision} = this.state;
        return (
            <div className='common-form'>
                Set your bodie series:
                <Form onSubmit={this.submitModelSeriesForm} id="bodySeries">
                    <FormGroup row>
                        <Col >
                            <Label className="label light-purple" for="modelSeries">Mode Series:Revision</Label>
                        </Col>
                        <Col sm={5}>
                            <Input type="text" name="modelSeries" id="modelSeries" value={modelSeries}
                                   onChange={this.handleChange}/>
                        </Col>
                        <Col>
                            <Input type="text" name="revision" id="revision" value={revision}
                                   onChange={this.handleChange}/>
                        </Col>
                        <Col>
                            <Button color={"success"}>Set</Button>
                        </Col>
                    </FormGroup>
                </Form>
                {this.state.error && <Alert color="danger">{this.state.error}</Alert>}
            </div>
        )
    }

    renderSendSTelemetryForm() {
        const paramsValue = this.state.paramsValue;
        const {bodyID} = this.state;

        return (
            <div className='common-form'>
                <Form onSubmit={this.submitTelemetryForm} id="bodyModel">
                    <FormGroup row>
                        <Col >
                            <Label className="label light-purple" for="bodyID">Body ID</Label>
                        </Col>
                        <Col>
                            <Input type="text" name="bodyID" id="bodyID" value={bodyID}
                                   onChange={this.handleChange}/>
                        </Col>
                    </FormGroup>
                    <p>Upload your body telemetry manualy:</p>
                    {Object.keys(paramsValue).map(key =>
                        <FormGroup row>
                            <Label className="label light-purple" sm={3}>{key}</Label>
                            <Col sm={5}>
                                <Input id={key} type={key} name={key} value={paramsValue[key]}
                                       onChange={this.HandleRefValueChange}/>
                            </Col>
                        </FormGroup>
                    )}
                    <Button color={"success"}>Upload</Button>
                </Form>
                {this.state.error && <Alert color="danger">{this.state.error}</Alert>}
            </div>
        );
    }

    renderHealthCheck() {
        return this.state.loading
            ? <p><em>No Data</em></p>
            : this.renderResults(this.state.report);
    }

    renderResults(report) {
        return (
            <div>
                <h3 id="tabelLabel">Last Healthcheck Results </h3>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                    <tr>
                        <th>Parameter</th>
                        <th>Check Result</th>
                    </tr>
                    </thead>
                    <tbody>
                    {Object.keys(report.checkResults).map(key =>
                        <tr key={key}>
                            <td>{key}</td>
                            <td>{report.checkResults[key]}</td>
                        </tr>
                    )}
                    {report.error && <Alert color="danger">{report.error}</Alert>}
                    </tbody>
                </table>
            </div>
        );
    }

    async populateHealthCheckData() {
        const response = await fetch('api/healthcheck');
        if (response.ok) {
            const data = await response.json();
            console.log(data);
            this.setState({report: data, loading: false});
        }
    }
}