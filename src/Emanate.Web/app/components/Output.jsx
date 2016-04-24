import React from 'react';
import {compose} from 'redux';
import {connect} from 'react-redux';
import Editable from './Editable.jsx';
import * as outputActions from '../actions/outputs';

class Output extends React.Component {
  render() {
    const {output, profile, onDelete} = this.props;
    return (
        <div>
            <Editable editing={output.get('editing')}
                      value={output.get('name')}
                      onValueClick={this.activateOutputEdit}
                      onEdit={this.editOutput}
                      onDelete={onDelete.bind(null, output.get('id'))} />
            <div>{profile.get('name')}</div>
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
  connect((state, props) => ({
      profile: state.profiles.find(profile => profile.get('id') === props.output.get('profileId'))
  }), {
    ...outputActions
  })
)(Output);
