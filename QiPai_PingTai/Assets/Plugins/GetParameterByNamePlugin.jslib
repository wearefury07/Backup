 var GetParameterByNamePlugin = {  
     getParameterByName: function() {
	  	var query = window.location.search.substring(1);
        var buffer = _malloc(lengthBytesUTF8(query) + 1);
        writeStringToMemory(query, buffer);
        return buffer;
	}
};

mergeInto(LibraryManager.library, GetParameterByNamePlugin);

