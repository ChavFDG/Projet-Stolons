
window.PublicProducts = {};

PublicProducts = window.PublicProducts;

ProductTypesModel = Backbone.Collection.extend({

    url: "/api/ProductTypes",
    
    initialize: function() {
	this.fetch();
    }
});


ProductModel = Backbone.Model.extend({

    //Products id key is 'Id'
    idAttribute: "Id",

    defaults: {},

    unitsEnum: ["Kg", "L"],

    /*
      return the html string according to this product's sell type
     */
    getSellTypeString: function() {
	if (this.get("Type") == 0) {
	    return "Au poids";
	} else if (this.get("Type") == 1) {
	    return "À la pièce";
	} else if (this.get("Type") == 2) {
	    return  "Emballé";
	} else {
	    return "Error";
	}
    },

    getStockUnitString: function() {
	if (this.get("Type") == 1) {
	    return "Pièces";
	}
	var productUnit = this.get("ProductUnit");
	return this.unitsEnum[productUnit];
    },

    getUnitPriceString: function() {
	if (this.get("Type") == 1) {
	    return this.get("UnitPrice") + " € l'unité";
	}
	return this.get("UnitPrice") + " € pour " + this.get("QuantityStepString");
    },

    getVolumePriceString: function() {
	var productUnit = this.get("ProductUnit");
	return this.get("Price") + " € / " +  this.unitsEnum[productUnit];
    },

    getSellStepString: function() {
	if (this.get("Type") == 1) {
	    return " Pièce(s)";
	}
	var productUnit = this.get("ProductUnit"); 
    }
});

ProductsModel = Backbone.Collection.extend(
    {
	defaults: [],

	model: ProductModel,

	url: "/api/Products",

	initialize: function() {
	    this.fetch();
	}
    }
);

function initModels() {
    PublicProducts.ProductTypesModel = new ProductTypesModel();
    PublicProducts.ProductsModel = new ProductsModel();
}

/* ---------------------------  Bellow Views definitions -------------------- */

ProducerViewModal = Backbone.View.extend({

    el: "#producerModal",

    template: _.template($("#producerModalTemplate").html()),

    initialize: function() {
    },

    open: function(productId) {
	this.currentProducer = PublicProducts.ProductsModel.get(productId).get("Producer");
	this.renderModal();
    },

    onClose: function() {
	this.currentProducer = null;
	this.$el.off('hide.bs.modal');
	this.$el.empty();
    },

    render: function() {
	this.$el.html(this.template({producer: this.currentProducer}));
    },

    renderModal: function() {
	this.render();
	this.$el.modal({keyboard: true, show: true});
	this.$el.on('hide.bs.modal', _.bind(this.onClose, this));
    }
});

ProductModalView = Backbone.View.extend({

    el: "#productModal",

    template: _.template($("#productModalTemplate").html()),

    open: function(productId) {
	this.currentProduct = PublicProducts.ProductsModel.get(productId);
	this.renderModal();
    },

    onClose: function() {
	this.currentProduct = null;
	this.$el.off('hide.bs.modal');
	this.$el.empty();
    },

    render: function() {
	this.$el.html(this.template({product: this.currentProduct.toJSON(), productModel: this.currentProduct}));
    },

    renderModal: function() {
	this.render();
	this.$el.modal({keyboard: true, show: true});
	this.$el.on('hide.bs.modal', _.bind(this.onClose, this));
    }
});

FiltersView = Backbone.View.extend({

    el: "#filters",

    template: _.template($("#filtersTemplate").html()),

    initialize: function(args) {
	this.model = args.model;
	this.productsModel = args.productsModel;
	this.listenTo(this.model, 'sync change', this.render);
	this.selectedFamily = "Tous";
    },

    onOptionSelected: function(selectedData) {
	this.selectedFamily = selectedData.params.data.id || "Tous";
	this.filterProducts();
    },

    filterProducts: function() {
	var searchTerm = this.$("#search").val();
	this.productsModel.forEach(function(productModel) {
	    var product = productModel.toJSON();
	    if ((this.selectedFamily == "Tous" || (product.Familly && product.Familly.FamillyName == this.selectedFamily)) &&
		(_.isEmpty(searchTerm) || product.Name.toLowerCase().contains(searchTerm) || (product.Description && product.Description.toLowerCase().contains(searchTerm)))) {
		$("#product-" + product.Id).removeClass("hidden");
	    } else {
		$("#product-" + product.Id).removeClass("hidden").addClass("hidden");
	    }
	}, this);
    },

    selectElemTemplate: function(elem) {
	if (!elem.id) {
	    return elem.text;
	}
	var dataImage = $(elem.element).data("image");
	if (!dataImage) {
	    return elem.text;
	} else {
	    return $('<span class="select-option"><img src="' + dataImage +'" />' + $(elem.element).text() + '</span>');
	}
    },

    render: function() {
	this.$el.html(this.template({ productTypes: this.model.toJSON() }));
	this.$('#familiesDropDown').select2({
	    minimumResultsForSearch: Infinity,
	    templateResult: this.selectElemTemplate,
	    templateSelection: this.selectElemTemplate
	});
	this.$('#familiesDropDown').on("select2:select", _.bind(this.onOptionSelected, this));
	this.$('#search').on("input", _.bind(function() {
	    this.filterProducts();
	}, this));
    }
});

ProductView = Backbone.View.extend(
    {
	template: _.template($("#productTemplate").html()),

	initialize: function(args) {
	    this.productId = args.productId;
	    this.el = args.el;
	    this.model = args.model;
	    this.model.on("sync change", this.render, this);
	},

	render: function() {
	    this.$el.html(this.template({product: this.model.toJSON(), productModel: this.model}));
	}
    }
);

ProductsView = Backbone.View.extend(
    {
	el: "#products",

	template: _.template($("#productsTemplate").html()),

	initialize: function(args) {
	    this.model = args.model;
	    this.model.on("sync", this.render, this);
	    this.views = {};
	},

	render: function() {
	    this.$el.html(this.template({products: this.model.models}));
	    this.model.forEach(function(productModel) {
		var productView = new ProductView({
		    el: "#product-" + productModel.get("Id"),
		    model: productModel
		});
		this.views[productModel.get("Id")] = productView;
		productView.render();
	    }, this);
	}
    }
);

function initViews() {

    //make the modal view global
    window.ProductModalView = new ProductModalView();
    window.ProducerModalView = new ProducerViewModal();

    PublicProducts.FiltersView = new FiltersView({model: PublicProducts.ProductTypesModel, productsModel: PublicProducts.ProductsModel});
    PublicProducts.ProductsView = new ProductsView({model: PublicProducts.ProductsModel});
}

$(function() {
    initModels();
    initViews();
});
