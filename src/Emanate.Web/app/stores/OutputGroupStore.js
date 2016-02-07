import uuid from 'node-uuid';
import alt from '../libs/alt';
import OutputGroupActions from '../actions/OutputGroupActions';

class OutputGroupStore {
    constructor() {
        this.bindActions(OutputGroupActions);

        this.outputGroups = [];
    }
    create(outputGroup) {
        const outputGroups = this.outputGroups;

        outputGroup.id = uuid.v4();

        this.setState({
            outputGroups: outputGroups.concat(outputGroup)
        });
    }
    update(updatedOutputGroup) {
        const outputGroups = this.outputGroups.map(outputGroup => {
            if(outputGroup.id === updatedOutputGroup.id) {
                return Object.assign({}, outputGroup, updatedOutputGroup);
            }

            return outputGroup;
        });

        this.setState({outputGroups});
    }
    delete(id) {
        this.setState({
            outputGroups: this.outputGroups.filter(outputGroup => outputGroup.id !== id)
        });
    }
}

export default alt.createStore(OutputGroupStore, 'OutputGroupStore');
