import AltContainer from 'alt-container';
import React from 'react';

import OutputGroups from './OutputGroups.jsx';
import OutputGroupActions from '../actions/OutputGroupsActions';
import OutputGroupStore from '../stores/OutputGroupsStore';

export default class App extends React.Component {
    render() {
        return (
          <div>
            <button className="add-outputGroup" onClick={this.addOutputGroup}>+</button>
            <AltContainer stores={[OutputGroupStore]} inject={{outputGroups: () => OutputGroupStore.getState().outputGroups}}>
              <OutputGroups onEdit={this.editOutputGroup} onDelete={this.deleteOutputGroup} />
            </AltContainer>
          </div>
    );

    }

    addOutputGroup() {
        OutputGroupActions.create({name: 'New observation group'});
    }

    editOutputGroup(id, name) {
        OutputGroupActions.update({id, name});
    }

    deleteOutputGroup(id) {
        OutputGroupActions.delete(id);
    }
}
