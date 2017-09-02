var Backgrid = require('backgrid');

module.exports = Backgrid.Row.extend({
    className : 'manual-import-row',

    _originalInit : Backgrid.Row.prototype.initialize,
    _originalRender : Backgrid.Row.prototype.render,

    initialize : function () {
        this._originalInit.apply(this, arguments);

        this.listenTo(this.model, 'change', this._setError);
        this.listenTo(this.model, 'change', this._setClasses);
    },

    render : function () {
        this._originalRender.apply(this, arguments);
        this._setError();
        this._setClasses();

        return this;
    },

    _setError : function () {
        if (this.model.has('artist') &&
            this.model.has('album') &&
            (this.model.has('tracks') && this.model.get('tracks').length > 0)&&
            this.model.has('quality')) {
            this.$el.removeClass('manual-import-error');
        }

        else {
            this.$el.addClass('manual-import-error');
        }
    },

    _setClasses : function () {
        this.$el.toggleClass('has-artist', this.model.has('artist'));
        this.$el.toggleClass('has-album', this.model.has('album'));
    }
});