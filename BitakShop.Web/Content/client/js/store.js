
if (document.readyState == 'loading') {
    document.addEventListener('DOMContentLoaded', ready)
} else {
    ready()
}


function ready() {


    var removeCartItemButtons = $(".close-cart");

    for (var i = 0; i < removeCartItemButtons.length; i++) {
        var button = removeCartItemButtons[i]
        button.addEventListener('click', removeCartItem)
    }

    
    var quantityInputs = document.getElementsByClassName('qty')
    for (var i = 0; i < quantityInputs.length; i++) {
        var input = quantityInputs[i]
        input.addEventListener('change', quantityChangedmincart)
    }



    var addToCartButtons = document.getElementsByClassName('add-to-cart')
    cartItems = document.getElementsByClassName('cart-list')[0]
   

    
    for (var i = 0; i < addToCartButtons.length; i++) {
        var button = addToCartButtons[i]
        button.addEventListener('click', addToCartClicked)
        
       
  
    }


    // ================================cart===========
    var removeCartItemButtonsForCart = $(".cart-remove-item");
    

    for (var i = 0; i < removeCartItemButtonsForCart.length; i++) {
        var button = removeCartItemButtonsForCart[i]
        button.addEventListener('click', removeCartItemForCart)
    }


    var quantityInputs = document.getElementsByClassName('quantity_cart')
    for (var i = 0; i < quantityInputs.length; i++) {
        var input = quantityInputs[i]
        input.addEventListener('change', quantityChanged)
    }

    
 


 
    
    



}






count= document.getElementsByClassName("cart-notification")[0]
    



function addToCartClicked(event) {

    
    // console.log(count)



    
    // countproduct = parseInt(count)
    
    
    
    
    var button = event.target
    var shopItem = button.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement
    title = shopItem.getElementsByClassName('product-title')[0].innerText
   
    var price = shopItem.getElementsByClassName('price')[0].innerText
    
 
    var imageSrc = shopItem.getElementsByClassName('cart-item-image')[0].src
    addItemToCart(title, price, imageSrc)
    updateCartTotal()
   
}

function addItemToCart(title, price, imageSrc) {
    var cartRow = document.createElement('li')
    
    cartRow.classList.add('mini_cart_item')
   
    
    
    var cartItemNames = cartItems.getElementsByClassName('cartproductname')
    for (var i = 0; i < cartItemNames.length; i++) {
        if (cartItemNames[i].innerText == title) {
            alert('این محصول قبلا به سبد خرید اضافه شده است')
          
            return false ;
        }
    
    }
    var cartRowContents = `
    
            <a class="close-cart"><i class="fa fa-times-circle"></i></a>
                                                <figure>
                                                    <a href="#" class="pull-right">
                                                        <img alt="" src="${imageSrc}">
                                                    </a>
                                                    <figcaption>
                                                                    <span>
                                                                        <a class="cartproductname"  href="shop-list.html">${title}</a>
                                                                    </span>
                                                        <p class="cart-price">${price}</p>
                                                        <div class="product-qty">
                                                            <label>تعداد</label>
                                                            <div class="custom-qty">
                                                                <input type="number" name="qty" maxlength="8" value="1"
                                                                       title="Qty" class="input-text qty">
                                                            </div>
                                                        </div>
                                                    </figcaption>
                                                </figure>

        `
    cartRow.innerHTML = cartRowContents
    cartItems.append(cartRow)
    
  
    
    
    

    cartRow.getElementsByClassName('close-cart')[0].addEventListener('click', removeCartItem)
    cartRow.getElementsByClassName('qty')[0].addEventListener('change', quantityChangedmincart)
    addcountproduct()
    document.body.scrollTop = 0; // For Safari
    document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
    
}




function removeCartItem(event) {
    var buttonClicked = event.target
    buttonClicked.parentElement.parentElement.remove()
    updateCartTotal()
    reducecountproduct()
}








function updateCartTotal() {
   
    var cartItemContainer = document.getElementsByClassName('cart-list')[0]
    var cartRows = cartItemContainer.getElementsByClassName('mini_cart_item')
    var total = 0
    for (var i = 0; i < cartRows.length; i++) {
        var cartRow = cartRows[i]
        var priceElement = cartRow.getElementsByClassName('cart-price')[0]
        var quantityElement = cartRow.getElementsByClassName('qty')[0]
        var price = parseFloat(priceElement.innerText.replace(' تومان', ''))
        var quantity = quantityElement.value
        total = total + (price * quantity)
    }
    total = Math.round(total * 100) / 100
    document.getElementsByClassName('cart-total-price')[0].innerText = ' تومان' + total

}


function quantityChangedmincart(event) {
     
    var input = event.target
    if (isNaN(input.value) || input.value <= 0) {
        input.value = 1
    }
    updateCartTotal()
}


function addcountproduct()
{
    

  

    countproductnumber = parseInt(count.innerText)
    


    countproductnumber = countproductnumber +1


    result = countproductnumber.toString()


    count.innerText = result




 
    

}

function reducecountproduct()
{

    countproductnumber = parseInt(count.innerText)


    countproductnumber = countproductnumber -1


    result = countproductnumber.toString()
 

    count.innerText = result

 
}




// ======================cart====================


function removeCartItemForCart(event) {
    var buttonClicked = event.target
    buttonClicked.parentElement.parentElement.parentElement.remove()
    updateCartTotalCart()
  
}

// $('.myremovebtn').click(function (event) {
//     event.preventDefault();
   
//   });


  function quantityChanged(event) {
     
    var input = event.target
    if (isNaN(input.value) || input.value <= 0) {
        input.value = 1
    }
    updateCartTotalCart()
}





  function updateCartTotalCart() {
   
    var cartItemContainer = document.getElementsByClassName('cartItemContainer')[0]
    var cartRows = cartItemContainer.getElementsByClassName('cartRows')
    var total = 0
    for (var i = 0; i < cartRows.length; i++) {
        var cartRow = cartRows[i]
        var priceElement = cartRow.getElementsByClassName('price')[0]
    
        var quantityElement = cartRow.getElementsByClassName('quantity_cart')[0]
       
        var price = parseFloat(priceElement.innerText.replace(' تومان', ''))
        var quantity = parseInt(quantityElement.value)
      
        total = total + (price * quantity)
    }
    total = Math.round(total * 100) / 100
  
   
    document.getElementsByClassName('pricecarttotal')[0].innerText = ' تومان' + total
    document.getElementsByClassName('pricecarttotal')[1].innerText = ' تومان' + total

}
