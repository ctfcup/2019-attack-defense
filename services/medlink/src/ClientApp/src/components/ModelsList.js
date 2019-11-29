import React, { Component } from 'react';

export class ModelsList extends Component {
  static displayName = ModelsList.name;

  constructor(props) {
    super(props);
    this.state = {models: [], loading: true, error: null, deviceId: ''};

    this.handleChange = this.handleChange.bind(this);
  }

  componentDidMount() {
    this.populateHealthCheckData();
  }

  handleChange = async (event) => {
    const {target} = event;
    const {name} = target;
    await this.setState({
      [name]: target.value,
    });
  };

  static renderResults(models) {
    return (
        <div>
            <p>List of supported body series</p>
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                <tr>
                    <th>Series</th>
                    <th>Revision</th>
                </tr>
                </thead>
                <tbody>
                {models.map(key=>
                    <tr key={key}>
                        <td>{key.series}</td>
                        <td>{key.revision}</td>
                    </tr>
                )}
                </tbody>
            </table>
        </div>
    );
  }

  render() {
    let contents = this.state.loading
        ? <p><em>No supported body models</em></p>
        : ModelsList.renderResults(this.state.models);

    return (
        <div>
          {contents}
        </div>
    );
  }

  async populateHealthCheckData() {
    const response = await fetch('api/bodymodels');
      if (response.ok) {
        const data = await response.json();
        this.setState({models: data, loading: false});
    }
  }
}
