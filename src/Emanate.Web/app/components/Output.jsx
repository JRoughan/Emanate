import React from 'react';
import {compose} from 'redux';
import {connect} from 'react-redux';
import Dropdown from 'react-dropdown';
import Editable from './Editable.jsx';
import * as outputActions from '../actions/outputs';

class Output extends React.Component {
  render() {
    const {output, profile, profileOptions, onDelete} = this.props;
    return (
        <div className="output">
            <Editable editing={output.get('editing')}
                      value={output.get('name')}
                      onValueClick={this.activateOutputEdit}
                      onEdit={this.setName}
                      onDelete={onDelete.bind(null, output.get('id'))} />
            <Dropdown options={profileOptions} onChange={this.setProfile} placeholder="Select a profile" />
            <div>{profile.get('name')}</div>
        </div>
    );
  }

  setName = (name) => {
      const outputId = this.props.output.get('id');
      this.props.updateOutput({ id: outputId, name, editing: false });
  };

  activateOutputEdit = () => {
      const outputId = this.props.output.get('id');
      this.props.updateOutput({ id: outputId, editing: true });
  };

  setProfile = (option) => {
      const outputId = this.props.output.get('id');
      this.props.updateOutput({ id: outputId, profileId: option.value });
  };
}

export default compose(
  connect((state, props) => ({
      profile: state.profiles.find(profile => profile.get('id') === props.output.get('profileId')),
      profileOptions: state.profiles.toJS().map(
          profile => {
              return {
                  value: profile.id,
                  label: profile.name
              }
          })
  }), {
    ...outputActions
  })
)(Output);
