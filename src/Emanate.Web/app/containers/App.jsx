import React from 'react';
import {connect} from 'react-redux';
import OutputGroups from '../components/OutputGroups.jsx';
import {createOutputGroup} from '../actions/outputGroups';
import {createProfile} from '../actions/profiles';

@connect((state) => ({
  outputGroups: state.outputGroups
}), {
  createOutputGroup, createProfile
})
export default class App extends React.Component {
  render() {
     const {outputGroups, createOutputGroup, createProfile} = this.props;

    return (
      <div>
        <button className="add-outputGroup" onClick={createOutputGroup.bind(null, { name: 'New outputGroup' })}>Add Output Group</button>
        <button className="add-profile" onClick={createProfile.bind(null, { name: 'New profile' })}>Add Profile</button>
        <OutputGroups outputGroups={outputGroups} />
      </div>
    );
  }
}
