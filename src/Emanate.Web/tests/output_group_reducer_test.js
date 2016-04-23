import assert from 'assert';
import {List} from 'immutable';
import * as types from 'app/actions/outputGroups';
import reducer from 'app/reducers/outputGroups';

describe('OutputGroupReducer', () => {
  it('should return the initial state', () => {
    assert.equal(reducer(undefined, {}).count(), 0);
  });

  it('should create outputGroups', () => {
    const outputGroup = {
      id: 'foobar',
      name: 'demo outputGroup',
      outputs: List()
    };
    const result = reducer(undefined, {
      type: types.CREATE_OUTPUTGROUP,
      outputGroup: outputGroup
    }).get(0);

    assert.equal(result.get('id'), outputGroup.id);
    assert.equal(result.get('name'), outputGroup.name);
    assert.equal(result.get('outputs').count(), 0);
  });

  it('should update outputGroups', () => {
    const outputGroup = {
      id: 'foobar',
      name: 'demo outputGroup',
      outputs: List()
    };
    const updatedName = 'foofoo';

    let outputGroups = reducer(undefined, {
      type: types.CREATE_OUTPUTGROUP,
      outputGroup: outputGroup
    });
    outputGroups = reducer(outputGroups, {
      type: types.UPDATE_OUTPUTGROUP,
      id: outputGroup.id,
      name: updatedName
    });

    assert.equal(outputGroups.count(), 1);
    assert.equal(outputGroups.get(0).get('id'), outputGroup.id);
    assert.equal(outputGroups.get(0).get('name'), updatedName);
    assert.equal(outputGroups.get(0).get('outputs').count(), 0);
  });

  it('should not crash while updating a non-existent outputGroup', () => {
    const outputGroup = {
      id: 'foobar',
      name: 'demo outputGroup',
      outputs: List()
    };

    let outputGroups = reducer(undefined, {
      type: types.CREATE_OUTPUTGROUP,
      outputGroup: outputGroup
    });
    outputGroups = reducer(outputGroups, {
      type: types.UPDATE_OUTPUTGROUP,
      id: outputGroup.id + outputGroup.id,
      name: 'foo'
    });

    assert.equal(outputGroups.count(), 1);
    assert.equal(outputGroups.get(0).get('id'), outputGroup.id);
    assert.equal(outputGroups.get(0).get('name'), outputGroup.name);
  });

  it('should delete outputGroups', () => {
    const outputGroup = {
      id: 'foobar',
      name: 'demo outputGroup',
      outputs: List()
    };

    let outputGroups = reducer(undefined, {
      type: types.CREATE_OUTPUTGROUP,
      outputGroup: outputGroup
    });
    outputGroups = reducer(outputGroups, {
      type: types.DELETE_OUTPUTGROUP,
      id: outputGroup.id
    });

    assert.equal(outputGroups.count(), 0);
  });


  it('should not crash while deleting a non-existent outputGroup', () => {
    const outputGroup = {
      id: 'foobar',
      name: 'demo outputGroup',
      outputs: List()
    };

    let outputGroups = reducer(undefined, {
      type: types.CREATE_OUTPUTGROUP,
      outputGroup: outputGroup
    });
    outputGroups = reducer(outputGroups, {
      type: types.DELETE_OUTPUTGROUP,
      id: outputGroup.id + outputGroup.id,
      name: 'foo'
    });

    assert.equal(outputGroups.count(), 1);
    assert.equal(outputGroups.get(0).get('id'), outputGroup.id);
    assert.equal(outputGroups.get(0).get('name'), outputGroup.name);
  });

  it('should attach outputs to outputGroup', () => {
    const outputGroup = {
      id: 'foobar',
      name: 'demo outputGroup',
      outputs: List()
    };
    const outputId = '123456';

    let outputGroups = reducer(undefined, {
      type: types.CREATE_OUTPUTGROUP,
      outputGroup: outputGroup
    });
    outputGroups = reducer(outputGroups, {
      type: types.ATTACH_TO_OUTPUTGROUP,
      outputGroupId: outputGroup.id,
      outputId: outputId
    });

    assert.equal(outputGroups.get(0).get('outputs').get(0), outputId);
  });

  it('should attach multiple outputs to outputGroup', () => {
    const outputGroup = {
      id: 'foobar',
      name: 'demo outputGroup',
      outputs: List()
    };
    const outputId = '123456';

    let outputGroups = reducer(undefined, {
      type: types.CREATE_OUTPUTGROUP,
      outputGroup: outputGroup
    });
    outputGroups = reducer(outputGroups, {
      type: types.ATTACH_TO_OUTPUTGROUP,
      outputGroupId: outputGroup.id,
      outputId: outputId
    });
    outputGroups = reducer(outputGroups, {
      type: types.ATTACH_TO_OUTPUTGROUP,
      outputGroupId: outputGroup.id,
      outputId: outputId + outputId
    });

    assert.equal(outputGroups.get(0).get('outputs').count(), 2);
    assert.equal(outputGroups.get(0).get('outputs').get(0), outputId);
    assert.equal(outputGroups.get(0).get('outputs').get(1), outputId + outputId);
  });

  it('should allow only one unique output per outputGroups when attaching', () => {
    const outputGroup1 = {
      id: 'foobar',
      name: 'demo outputGroup',
      outputs: List()
    };
    const outputGroup2 = {
      id: 'foobar2',
      name: 'demo outputGroup 2',
      outputs: List()
    };
    const outputId = '123456';

    let outputGroups = reducer(undefined, {
      type: types.CREATE_OUTPUTGROUP,
      outputGroup: outputGroup1
    });
    outputGroups = reducer(outputGroups, {
      type: types.CREATE_OUTPUTGROUP,
      outputGroup: outputGroup2
    });
    outputGroups = reducer(outputGroups, {
      type: types.ATTACH_TO_OUTPUTGROUP,
      outputGroupId: outputGroup1.id,
      outputId: outputId
    });
    outputGroups = reducer(outputGroups, {
      type: types.ATTACH_TO_OUTPUTGROUP,
      outputGroupId: outputGroup2.id,
      outputId: outputId
    });

    assert.equal(outputGroups.get(0).get('outputs').count(), 0);
    assert.equal(outputGroups.get(1).get('outputs').get(0), outputId);
  });

  it('should detach outputs to outputGroups', () => {
    const outputGroup = {
      id: 'foobar',
      name: 'demo outputGroup',
      outputs: List()
    };
    const outputId = '123456';

    let outputGroups = reducer(undefined, {
      type: types.CREATE_OUTPUTGROUP,
      outputGroup: outputGroup
    });
    outputGroups = reducer(outputGroups, {
      type: types.DETACH_FROM_OUTPUTGROUP,
      outputGroupId: outputGroup.id,
      outputId: outputId
    });

    assert.equal(outputGroups.get(0).get('outputs').count(), 0);
  });

});
