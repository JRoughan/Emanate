import assert from 'assert';
import Immutable from 'immutable';

describe('Discovery', () => {
  it('should map JS object from immutable list', () => {
      const input = [
          { id: 'a', name: 'Apple' },
          { id: 'b', name: 'Banana' },
          { id: 'c', name: 'Carrot' }
      ];

    var list = Immutable.fromJS(input);

    var result = list.toJSON().map(item => {
          return {
              id: item.id,
              label: item.name
          }
      });

      assert.equal(result.length, 3);
      assert.equal(result[1].id, "b");
      assert.equal(result[1].label, "Banana");
  });
});
