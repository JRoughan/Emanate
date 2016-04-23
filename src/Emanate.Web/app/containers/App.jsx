import React from 'react';
import {connect} from 'react-redux';
import OutputGroups from '../components/OutputGroups.jsx';
import {createOutputGroup} from '../actions/outputGroups';

@connect((state) => ({
  outputGroups: state.outputGroups
}), {
  createOutputGroup
})
export default class App extends React.Component {
  render() {
    const {outputGroups, createOutputGroup} = this.props;

    return (
      <div>
        <button className="add-outputGroup"
          onClick={createOutputGroup.bind(null, {
            name: 'New outputGroup'
          })}>+</button>
        <OutputGroups outputGroups={outputGroups} />
      </div>
    );
  }
}
