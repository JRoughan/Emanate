import uuid from 'node-uuid';
import React from 'react';
import OutputGroups from './OutputGroups.jsx';

export default class App extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            outputGroups: [
              {
                  id: uuid.v4(),
                  name: 'Cygnus'
              },
              {
                  id: uuid.v4(),
                  name: 'Draco'
              },
              {
                  id: uuid.v4(),
                  name: 'Carina'
              }
            ]
        };
    }
    render() {
        const outputGroups = this.state.outputGroups;

        return (
          <div>
            <button className="add-outputGroup" onClick={this.addOutputGroup}>+</button>
            <OutputGroups outputGroups={outputGroups}
              onEdit={this.editOutputGroup}
              onDelete={this.deleteOutputGroup} />
          </div>
        );
    }

    deleteOutputGroup = (id) => {
        this.setState({
            outputGroups: this.state.outputGroups.filter(outputGroup => outputGroup.id !== id)
        });
    };

    addOutputGroup = () => {
        this.setState({
            outputGroups: this.state.outputGroups.concat([{
                id: uuid.v4(),
                name: 'New output group'
            }])
        });
    };

    editOutputGroup = (id, name) => {
        const outputGroups = this.state.outputGroups.map(outputGroup => {
            if(outputGroup.id === id && name) {
                outputGroup.name = name;
            }
    
            return outputGroup;
        });
    
        this.setState({outputGroups});
    };
}
