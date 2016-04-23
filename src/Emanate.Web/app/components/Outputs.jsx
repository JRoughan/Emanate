import React from 'react';
import Output from './Output.jsx';

export default class Outputs extends React.Component {
  render() {
    const {outputs, onDelete} = this.props;

    return (
      <ul className="outputs">{outputs.map((output) =>
        <Output key={output.get('id')} output={output} onDelete={onDelete} className="output" />
    )}</ul>);
  }
}
