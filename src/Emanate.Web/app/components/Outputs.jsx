import React from 'react';
import {connect} from 'react-redux';
import Editable from './Editable.jsx';
import Output from './Output.jsx';
import {move} from '../actions/outputGroups';

class Outputs extends React.Component {
  render() {
    const {outputs, move, onValueClick, onEdit, onDelete} = this.props;

    return (<ul className="outputs">{outputs.map((output) =>
      <Output className="output" id={output.get('id')} key={output.get('id')}
        editing={output.get('editing')} onMove={move}>
        <Editable
          editing={output.get('editing')}
          value={output.get('name')}
          onValueClick={onValueClick.bind(null, output.get('id'))}
          onEdit={onEdit.bind(null, output.get('id'))}
          onDelete={onDelete.bind(null, output.get('id'))} />
      </Output>
    )}</ul>);
  }
}

export default connect(() => ({}), {
  move
})(Outputs);
