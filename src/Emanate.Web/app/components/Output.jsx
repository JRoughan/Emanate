import React from 'react';
import {compose} from 'redux';
import {connect} from 'react-redux';
import Editable from './Editable.jsx';
import * as outputActions from '../actions/outputs';

class Output extends React.Component {
  render() {
    const {output, onDelete} = this.props;
    return (
        <div>
            <Editable editing={output.get('editing')}
                      value={output.get('name')}
                      onValueClick={this.activateOutputEdit}
                      onEdit={this.editOutput}
                      onDelete={onDelete.bind(null, output.get('id'))} />
        </div>
    );
  }

  editOutput = (name) => {
      const outputId = this.props.output.get('id');
      this.props.updateOutput({ id: outputId, name, editing: false });
  };
  
  activateOutputEdit = () => {
      const outputId = this.props.output.get('id');
      this.props.updateOutput({ id: outputId, editing: true });
  };
}

export default compose(
  connect(() => ({}), {
    ...outputActions
  })
)(Output);
