import AltContainer from 'alt-container';
import React from 'react';

import OutputGroups from './OutputGroups.jsx';
import OutputGroupActions from '../actions/OutputGroupActions';
import OutputGroupStore from '../stores/OutputGroupStore';

export default class App extends React.Component {
  render() {
    return (
      <div>
        <button className="add-outputGroup" onClick={this.addOutputGroup}>+</button>
        <AltContainer stores={[OutputGroupStore]} inject={{ outputGroups: () => OutputGroupStore.getState().outputGroups || [] }} >
          <OutputGroups />
        </AltContainer>
      </div>
    );
  }
  addOutputGroup() {
    OutputGroupActions.create({name: 'New output group'});
  }
}
